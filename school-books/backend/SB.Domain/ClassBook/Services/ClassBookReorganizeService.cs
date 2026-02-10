namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShiftVO = IShiftsQueryRepository.GetVO;

internal class ClassBookReorganizeService : IClassBookReorganizeService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IClassBookService classBookService;
    private readonly IClassBooksAggregateRepository classBookAggregateRepository;
    private readonly IClassBooksQueryRepository classBooksQueryRepository;
    private readonly IClassGroupsQueryRepository classGroupsQueryRepository;
    private readonly ISchoolYearSettingsAggregateRepository schoolYearSettingsAggregateRepository;
    private readonly IOffDaysAggregateRepository offDaysAggregateRepository;
    private readonly IClassBookOffDayDatesAggregateRepository classBookOffDayDatesAggregateRepository;
    private readonly ISchedulesAggregateRepository scheduleAggregateRepository;
    private readonly IShiftsQueryRepository shiftsQueryRepository;
    private readonly IScopedAggregateRepository<Shift> shiftAggregateRepository;

    public ClassBookReorganizeService(
        IUnitOfWork unitOfWork,
        IClassBookService classBookService,
        IClassBooksAggregateRepository classBookAggregateRepository,
        IClassBooksQueryRepository classBooksQueryRepository,
        IClassGroupsQueryRepository classGroupsQueryRepository,
        ISchoolYearSettingsAggregateRepository schoolYearSettingsAggregateRepository,
        IOffDaysAggregateRepository offDaysAggregateRepository,
        IClassBookOffDayDatesAggregateRepository classBookOffDayDatesAggregateRepository,
        ISchedulesAggregateRepository scheduleAggregateRepository,
        IShiftsQueryRepository shiftsQueryRepository,
        IScopedAggregateRepository<Shift> shiftAggregateRepository)
    {
        this.unitOfWork = unitOfWork;
        this.classBookService = classBookService;
        this.classBookAggregateRepository = classBookAggregateRepository;
        this.classBooksQueryRepository = classBooksQueryRepository;
        this.classGroupsQueryRepository = classGroupsQueryRepository;
        this.schoolYearSettingsAggregateRepository = schoolYearSettingsAggregateRepository;
        this.classBookOffDayDatesAggregateRepository = classBookOffDayDatesAggregateRepository;
        this.offDaysAggregateRepository = offDaysAggregateRepository;
        this.scheduleAggregateRepository = scheduleAggregateRepository;
        this.shiftsQueryRepository = shiftsQueryRepository;
        this.shiftAggregateRepository = shiftAggregateRepository;
        this.scheduleAggregateRepository = scheduleAggregateRepository;
    }

    public async Task<int> CombineClassBooks(
        int schoolYear,
        int instId,
        int parentClassId,
        string? parentClassBookName,
        int? childClassIdForDataTransfer,
        int sysUserId,
        CancellationToken ct)
    {
        // Mark old classBooks as invalid
        var classGroups = await this.classGroupsQueryRepository.GetClassGroupsAsync(schoolYear, instId, new[] { parentClassId }, ct);
        var childClassIds = classGroups
            .Where(cg => cg.ClassGroup.ParentClassId != null)
            .Select(cg => cg.ClassGroup.ClassId)
            .ToArray();
        var childClassBooks =
            (await this.classBookAggregateRepository
                .FindAllByClassIdsAsync(schoolYear, childClassIds, ct))
            .Where(cb => cb.IsValid)
            .ToArray();
        Array.ForEach(childClassBooks, c => c.MarkAsInvalid(sysUserId));

        var ids = await this.classBookService.CreateClassBooks(schoolYear, instId, new[] { (parentClassId, parentClassBookName) }, sysUserId, ct);
        var newClassBookId = ids.First();

        if (childClassIdForDataTransfer == null)
        {
            return newClassBookId;
        }

        var childClassBookId = childClassBooks.First(c => c.ClassId == childClassIdForDataTransfer).ClassBookId;

        // Add new ClassBook to SchoolYearSettings
        var schoolYearSettings = await this.schoolYearSettingsAggregateRepository.FindAllByClassBookAsync(schoolYear, instId, childClassBookId, ct);
        if (schoolYearSettings.Any())
        {
            foreach (var schoolYearSetting in schoolYearSettings)
            {
                schoolYearSetting.AddClassBookId(newClassBookId);
            }
        }

        // Add new ClassBook to OffDays
        var offDays = await this.offDaysAggregateRepository.FindAllByClassBookAsync(schoolYear, instId, childClassBookId, ct);
        var classBookOffDayDates = await this.classBookOffDayDatesAggregateRepository.FindAllByClassBookAsync(schoolYear, childClassBookId, ct);
        if (offDays.Any())
        {
            foreach (var offDay in offDays)
            {
                offDay.AddOffDayToClassBook(newClassBookId);

                var cbOffDates = classBookOffDayDates.Where(cbodd => cbodd.OffDayId == offDay.OffDayId).ToArray();
                foreach (var cbOffDate in cbOffDates)
                {
                    await this.classBookOffDayDatesAggregateRepository
                        .AddAsync(
                            new ClassBookOffDayDate(
                                schoolYear,
                                newClassBookId,
                                cbOffDate.Date,
                                offDay.OffDayId,
                                offDay.IsPgOffProgramDay),
                            false,
                            ct);
                }
            }
        }

        // Copy schedule to the new classBook
        var schedules = await this.scheduleAggregateRepository.FindAllByClassBookAsync(schoolYear, childClassBookId, ct);
        var offDaysDates = classBookOffDayDates.Select(cbodd => cbodd.Date).ToArray();
        foreach (var schedule in schedules)
        {
            int? newShiftId = null;
            var existingShift = await this.shiftsQueryRepository.GetAsync(schoolYear, schedule.ShiftId, ct);
            if (existingShift.IsAdhoc)
            {
                newShiftId = await this.GenerateNewShift(schoolYear, instId, sysUserId, existingShift, ct);
            }

            var newSchedule = schedule.CopyScheduleToNewClassBook(newClassBookId, offDaysDates, newShiftId);

            await this.scheduleAggregateRepository.AddAsync(newSchedule, ct);
        }

        await this.unitOfWork.SaveAsync(ct);

        return newClassBookId;
    }

    public async Task<int[]> SeparateClassBooks(
        int schoolYear,
        int instId,
        int parentClassId,
        (int classId, string? classBookName)[] childClassBooks,
        int sysUserId,
        CancellationToken ct)
    {
        // Mark old classBook as invalid
        var parentClassBook =
            (await this.classBookAggregateRepository
                .FindAllByClassIdsAsync(
                    schoolYear,
                    new[] { parentClassId },
                    ct))
            .First(cb => cb.IsValid);
        parentClassBook.MarkAsInvalid(sysUserId);

        var classBookIds = await this.classBookService.CreateClassBooks(schoolYear, instId, childClassBooks, sysUserId, ct);

        var parentClassBookId = parentClassBook.ClassBookId;

        // Add new ClassBook to SchoolYearSettings
        var schoolYearSettings = await this.schoolYearSettingsAggregateRepository.FindAllByClassBookAsync(schoolYear, instId, parentClassBookId, ct);
        if (schoolYearSettings.Any())
        {
            foreach (var classBookId in classBookIds)
            {
                foreach (var schoolYearSetting in schoolYearSettings)
                {
                    schoolYearSetting.AddClassBookId(classBookId);
                }
            }
        }

        // Add new ClassBook to OffDays
        var offDays = await this.offDaysAggregateRepository.FindAllByClassBookAsync(schoolYear, instId, parentClassBookId, ct);
        var classBookOffDayDates = await this.classBookOffDayDatesAggregateRepository.FindAllByClassBookAsync(schoolYear, parentClassBookId, ct);
        if (offDays.Any())
        {
            foreach (var classBookId in classBookIds)
            {
                foreach (var offDay in offDays)
                {
                    offDay.AddOffDayToClassBook(classBookId);

                    var cbOffDates = classBookOffDayDates.Where(cbodd => cbodd.OffDayId == offDay.OffDayId).ToArray();
                    foreach (var cbOffDate in cbOffDates)
                    {
                        await this.classBookOffDayDatesAggregateRepository
                            .AddAsync(
                                new ClassBookOffDayDate(
                                    schoolYear,
                                    classBookId,
                                    cbOffDate.Date,
                                    offDay.OffDayId,
                                    offDay.IsPgOffProgramDay),
                            false,
                                ct);
                    }
                }
            }
        }

        // Copy schedule to the new classBook
        var schedules = await this.scheduleAggregateRepository.FindAllByClassBookAsync(schoolYear, parentClassBookId, ct);
        var offDaysDates = classBookOffDayDates.Select(cbodd => cbodd.Date).ToArray();
        foreach (var classBookId in classBookIds)
        {
            foreach (var schedule in schedules)
            {
                if (schedule.IsIndividualSchedule &&
                    (await this.classBooksQueryRepository.GetStudentsAsync(schoolYear, classBookId, ct)).Any(s => s.PersonId == schedule.PersonId))
                {
                    continue;
                }

                int? newShiftId = null;
                var existingShift = await this.shiftsQueryRepository.GetAsync(schoolYear, schedule.ShiftId, ct);
                if (existingShift.IsAdhoc)
                {
                    newShiftId = await this.GenerateNewShift(schoolYear, instId, sysUserId, existingShift, ct);
                }

                var newSchedule = schedule.CopyScheduleToNewClassBook(classBookId, offDaysDates, newShiftId);
                await this.scheduleAggregateRepository.AddAsync(newSchedule, ct);
            }
        }

        await this.unitOfWork.SaveAsync(ct);

        return classBookIds;
    }

    private async Task<int> GenerateNewShift(
        int schoolYear,
        int instId,
        int sysUserId,
        ShiftVO existingShift,
        CancellationToken ct)
    {
        var newShift = new Shift(
            schoolYear,
            instId,
            string.Empty,
            existingShift.IsMultiday,
            true,
            existingShift.Days.SelectMany(d => d.Hours.Select(h => (
                day: d.Day,
                hourNumber: h.HourNumber,
                startTime: h.StartTime.ToString("hh\\:mm"),
                endTime: h.EndTime.ToString("hh\\:mm")
            ))).ToArray(),
            sysUserId);
        await this.shiftAggregateRepository.AddAsync(newShift, ct);

        return newShift.ShiftId;
    }
}
