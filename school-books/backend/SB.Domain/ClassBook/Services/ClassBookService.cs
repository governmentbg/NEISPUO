namespace SB.Domain;

using Microsoft.Extensions.Logging;
using SB.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ClassBookService : IClassBookService
{
    private IUnitOfWork unitOfWork;
    private IScopedAggregateRepository<ClassBook> classBookAggregateRepository;
    private IScopedAggregateRepository<ClassBookSchoolYearSettings> classBookSchoolYearSettingsAggregateRepository;
    private IScopedAggregateRepository<ClassBookOffDayDate> classBookOffDayDatesAggregateRepository;
    private IClassGroupsQueryRepository classGroupsQueryRepository;
    private IClassBooksQueryRepository classBooksQueryRepository;
    private IStudentClassQueryRepository studentClassQueryRepository;
    private ISchoolYearSettingsProvider schoolYearSettingsProvider;
    private IOffDayRebuildService offDayRebuildService;
    private ILogger<ClassBookService> logger;

    public ClassBookService(
        IUnitOfWork unitOfWork,
        IScopedAggregateRepository<ClassBook> classBookAggregateRepository,
        IScopedAggregateRepository<ClassBookSchoolYearSettings> classBookSchoolYearSettingsAggregateRepository,
        IScopedAggregateRepository<ClassBookOffDayDate> classBookOffDayDatesAggregateRepository,
        IClassGroupsQueryRepository classGroupsQueryRepository,
        IClassBooksQueryRepository classBooksQueryRepository,
        IStudentClassQueryRepository studentClassQueryRepository,
        ISchoolYearSettingsProvider schoolYearSettingsProvider,
        IOffDayRebuildService offDayRebuildService,
        ILogger<ClassBookService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.classBookAggregateRepository = classBookAggregateRepository;
        this.classBookSchoolYearSettingsAggregateRepository = classBookSchoolYearSettingsAggregateRepository;
        this.classBookOffDayDatesAggregateRepository = classBookOffDayDatesAggregateRepository;
        this.classGroupsQueryRepository = classGroupsQueryRepository;
        this.classBooksQueryRepository = classBooksQueryRepository;
        this.studentClassQueryRepository = studentClassQueryRepository;
        this.schoolYearSettingsProvider = schoolYearSettingsProvider;
        this.offDayRebuildService = offDayRebuildService;
        this.logger = logger;
    }

    public async Task<int[]> CreateClassBooks(
        int schoolYear,
        int instId,
        (int classId, string? classBookName)[] classBooks,
        int sysUserId,
        CancellationToken ct)
    {
        var instType = await this.classGroupsQueryRepository.GetInstTypeAsync(
            schoolYear,
            instId,
            ct);

        var classGroupsResult = await this.classGroupsQueryRepository.GetClassGroupsAsync(
            schoolYear,
            instId,
            classBooks.Select(cb => cb.classId).ToArray(),
            ct);

        var classGroups = classGroupsResult
            .ToDictionary(cg => cg.ClassGroup.ClassId);
         var childClassGroups = classGroupsResult
            .Where(cg => cg.ClassGroup.ParentClassId != null)
            .ToLookup(cg => cg.ClassGroup.ParentClassId);

         int[] classBookIds = new int[classBooks.Length];
          List<(StudentClass studentClass, int? classNumber)> studentClassNumbers = new();
        for (int i = 0; i < classBooks.Length; i++)
        {
            int classId = classBooks[i].classId;
            string? classBookName = classBooks[i].classBookName;
            var classGroup = classGroups[classId];

            if (!classGroup.ClassGroup.IsValid)
            {
                throw new DomainValidationException($"Cannot create classBook for classGroup with IsValid=0. ClassId = {classGroup.ClassGroup.ClassId}.");
            }

            var classBook =
                new ClassBook(
                    schoolYear,
                    instId,
                    instType,
                    classGroup.ClassGroup,
                    classGroup.BasicClass,
                    classGroup.ClassType,
                    childClassGroups[classGroup.ClassGroup.ClassId]
                        .Select(ccg => (ccg.ClassGroup, (ClassType?)ccg.ClassType))
                        .ToArray(),
                    classBookName ??
                        ClassBook.GetSuggestedClassBookNameForClassGroup(classGroup.ClassGroup, classGroup.BasicClass) ??
                        string.Empty,
                    sysUserId);
            await this.classBookAggregateRepository.AddAsync(
                entity: classBook,
                preventDetectChanges: true,
                ct: ct);

            var settings =
                await this.schoolYearSettingsProvider.GetForClassBookAsync(
                    classBook.SchoolYear,
                    classBook.InstId,
                    classBook.ClassBookId,
                    classBook.BasicClassId,
                    childClassGroups[classGroup.ClassGroup.ClassId]
                        .Select(ccg => ccg.BasicClass?.BasicClassId)
                        .ToArray(),
                    ct);
            await this.classBookSchoolYearSettingsAggregateRepository.AddAsync(
                entity: new ClassBookSchoolYearSettings(
                    schoolYear,
                    classBook.ClassBookId,
                    settings.SchoolYearSettingsId,
                    settings.SchoolYearStartDateLimit,
                    settings.SchoolYearStartDate,
                    settings.FirstTermEndDate,
                    settings.SecondTermStartDate,
                    settings.SchoolYearEndDate,
                    settings.SchoolYearEndDateLimit,
                    settings.HasFutureEntryLock,
                    settings.PastMonthLockDay),
                preventDetectChanges: true,
                ct: ct);

            var classBookOffDayDates =
                await this.offDayRebuildService.CreateForClassBookAsync(
                    classBook.SchoolYear,
                    classBook.InstId,
                    classBook.ClassBookId,
                    classBook.BasicClassId,
                    ct);
            foreach (var classBookOffDayDate in classBookOffDayDates)
            {
                await this.classBookOffDayDatesAggregateRepository.AddAsync(
                    entity: classBookOffDayDate,
                    preventDetectChanges: true,
                    ct: ct);
            }

            var students =
                await this.studentClassQueryRepository.FindAllByClassBookAsync(
                    classBook.SchoolYear,
                    classBook.ClassId,
                    classBook.ClassIsLvl2,
                    ct
                );

            var currentClassNumber = 1;
            foreach (var studentClasses in students
                .GroupBy(s => new { s.PersonId, s.FirstName, s.MiddleName, s.LastName })
                .OrderBy(g => g.Key.FirstName)
                .ThenBy(g => g.Key.MiddleName)
                .ThenBy(g => g.Key.LastName)
                .Select(g => g.Select(gi => gi.StudentClass)))
            {
                if (studentClasses.Any(sc => sc.Status == StudentClassStatus.Enrolled))
                {
                    foreach (var sc in studentClasses)
                    {
                        studentClassNumbers.Add((sc, currentClassNumber));
                    }
                    currentClassNumber++;
                }
                else
                {
                    foreach (var sc in studentClasses)
                    {
                        studentClassNumbers.Add((sc, null));
                    }
                }
            }

            if (classBook.BookType == ClassBookType.Book_CSOP)
            {
                var curriculumIds =
                    await this.classBooksQueryRepository.GetCurriculumIdsForClassBookAsync(
                        classBook.SchoolYear,
                        classBook.ClassId,
                        classBook.ClassIsLvl2,
                        ct
                    );

                classBook.MarkAllStudentsAsSpecialNeeds(
                    curriculumIds,
                    students
                        .Select(s => s.StudentClass.PersonId)
                        .Distinct()
                        .ToArray());
            }

            classBookIds[i] = classBook.ClassBookId;
        }

        await this.unitOfWork.SaveAsync(ct);

        // Set class numbers after saving the rest of the entities
        // as we need the writes to the system versioned temporal table
        // StudentClass to be fast and retry them in case of a failure
        // due to a concurrent update to the same table.
        // There is no transaction wrapping the whole operation as
        // failing the class number update would not be a problem.
        // There are UI controls to fix the class numbers.

        foreach (var (sc, classNumber) in studentClassNumbers)
        {
            sc.SetClassNumber(classNumber);
        }

        try
        {
            await this.unitOfWork.SaveWithRetryingStrategyAsync(
                new []
                {
                    // see https://dba.stackexchange.com/questions/211467/error-updating-temporal-tables
                    SqlServerErrorCodes.DataModificationFailedOnSystemVersionedTable
                },
                ct);
        }
        catch (DomainUpdateSqlException ex)
        {
            this.logger.LogError(ex, "Failed to set class numbers after class books creation.");
        }

        return classBookIds;
    }
}
