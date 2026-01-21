namespace SB.Domain;

using Microsoft.Extensions.DependencyInjection;
using SB.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IClassBookStudentPrintRepository;

internal class ClassBookStudentPrintService : IPrintService
{
    private const int FirstClassBasicClassId = 1;

    private readonly IClassBookStudentPrintRepository classBookStudentPrintRepository;
    private readonly IClassBookPrintRepository classBookPrintRepository;
    private readonly ISchedulesQueryRepository schedulesQueryRepository;
    private readonly IHtmlTemplateService htmlTemplateService;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public ClassBookStudentPrintService(
        IClassBookStudentPrintRepository classBookStudentPrintRepository,
        IClassBookPrintRepository classBookPrintRepository,
        ISchedulesQueryRepository schedulesQueryRepository,
        IHtmlTemplateService htmlTemplateService,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.classBookStudentPrintRepository = classBookStudentPrintRepository;
        this.classBookPrintRepository = classBookPrintRepository;
        this.schedulesQueryRepository = schedulesQueryRepository;
        this.htmlTemplateService = htmlTemplateService;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task RenderHtmlAsync(string printParamsStr, TextWriter textWriter, CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookStudentPrintParams>(printParamsStr);

        var coverPageData =
            await this.classBookStudentPrintRepository.GetCoverPageDataAsync(
                printParams!.SchoolYear,
                printParams.ClassBookId,
                printParams.StudentPersonId,
                ct);

        string template = coverPageData.BookType switch
        {
            ClassBookType.Book_PG => HtmlTemplateConfig.ClassBookStudentPrint_Book_PG.TemplateName,
            ClassBookType.Book_I_III => HtmlTemplateConfig.ClassBookStudentPrint_Book_I_III.TemplateName,
            ClassBookType.Book_IV => HtmlTemplateConfig.ClassBookStudentPrint_Book_IV.TemplateName,
            ClassBookType.Book_V_XII => HtmlTemplateConfig.ClassBookStudentPrint_Book_V_XII.TemplateName,
            ClassBookType.Book_CDO => HtmlTemplateConfig.ClassBookStudentPrint_Book_CDO.TemplateName,
            ClassBookType.Book_DPLR => HtmlTemplateConfig.ClassBookStudentPrint_Book_DPLR.TemplateName,
            ClassBookType.Book_CSOP => HtmlTemplateConfig.ClassBookStudentPrint_Book_CSOP.TemplateName,
            _ => throw new DomainException("Unknown BookType"),
        };

        object model = await this.GetModelAsync(
            printParams.SchoolYear,
            printParams.ClassBookId,
            printParams.StudentPersonId,
            coverPageData,
            ct);

        await this.htmlTemplateService.RenderAsync(template, model, textWriter, ct);
    }

    public async Task FinalizePrintAsProcessedAsync(string printParamsStr, int printId, int blobId, string? contentHash, CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookStudentPrintParams>(printParamsStr);
        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var classBooksAggregateRepository = scope.ServiceProvider.GetRequiredService<IClassBooksAggregateRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var classBook = await classBooksAggregateRepository.FindAsync(printParams!.SchoolYear, printParams.ClassBookId, ct);

        classBook.ClassBookStudentPrintMarkProcessed(printId, blobId);

        await unitOfWork.SaveAsync(ct);
    }

    public async Task FinalizePrintAsErroredAsync(string printParamsStr, int printId, CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookStudentPrintParams>(printParamsStr);
        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var classBooksAggregateRepository = scope.ServiceProvider.GetRequiredService<IClassBooksAggregateRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var classBook = await classBooksAggregateRepository.FindAsync(printParams!.SchoolYear, printParams.ClassBookId, ct);

        classBook.ClassBookStudentPrintMarkErrored(printId);

        await unitOfWork.SaveAsync(ct);
    }

    private async Task<object> GetModelAsync(int schoolYear, int classBookId, int personId, GetStudentCoverPageDataVO coverPageData, CancellationToken ct)
    {
        (int instId, int classId, bool classIsLvl2) = await this.classBookPrintRepository.GetClassBookInfoAsync(schoolYear, classBookId, ct);

        switch (coverPageData.BookType)
        {
            case ClassBookType.Book_PG:
                return new Student_Book_PGModel(
                    this.GetCoverPageModel(schoolYear, coverPageData),
                    await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                    await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                    await this.GetAttendancesModelAsync(schoolYear, classBookId, personId, ct),
                    await this.GetPgResultsModelAsync(schoolYear, classBookId, personId, ct),
                    await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
            case ClassBookType.Book_I_III:
                {
                    var grades = await this.classBookStudentPrintRepository.GetGradesAsync(schoolYear, classBookId, personId, ct);
                    var absences = await this.classBookStudentPrintRepository.GetAbsencesAsync(schoolYear, classBookId, personId, ct);
                    var remarks = await this.classBookStudentPrintRepository.GetRemarksAsync(schoolYear, classBookId, personId, ct);
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);

                    return new Student_Book_I_IIIModel(
                        coverPageData.BasicClassId == FirstClassBasicClassId,
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ClassExam, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ControlExam, ct),
                        this.GetGradesModel(SchoolTerm.TermOne, grades),
                        this.GetRemarksModel(SchoolTerm.TermOne, remarks),
                        this.GetGradesModel(SchoolTerm.TermTwo, grades),
                        this.GetRemarksModel(SchoolTerm.TermTwo, remarks),
                        this.GetFinalGradesModel(grades),
                        coverPageData.BasicClassId == FirstClassBasicClassId
                            ? await this.GetFirstGradeResultsModelAsync(schoolYear, classBookId, personId, ct)
                            : new StudentFirstGradeResultsModel(null, null),
                        this.GetAbsencesModel(absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetSanctionsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
                }
            case ClassBookType.Book_IV:
                {
                    var grades = await this.classBookStudentPrintRepository.GetGradesAsync(schoolYear, classBookId, personId, ct);
                    var absences = await this.classBookStudentPrintRepository.GetAbsencesAsync(schoolYear, classBookId, personId, ct);
                    var remarks = await this.classBookStudentPrintRepository.GetRemarksAsync(schoolYear, classBookId, personId, ct);
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);

                    return new Student_Book_IVModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ClassExam, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ControlExam, ct),
                        this.GetGradesModel(SchoolTerm.TermOne, grades),
                        this.GetRemarksModel(SchoolTerm.TermOne, remarks),
                        this.GetGradesModel(SchoolTerm.TermTwo, grades),
                        this.GetRemarksModel(SchoolTerm.TermTwo, remarks),
                        this.GetFinalGradesModel(grades),
                        this.GetAbsencesModel(absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetSanctionsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
                }
            case ClassBookType.Book_V_XII:
                {
                    var grades = await this.classBookStudentPrintRepository.GetGradesAsync(schoolYear, classBookId, personId, ct);
                    var absences = await this.classBookStudentPrintRepository.GetAbsencesAsync(schoolYear, classBookId, personId, ct);
                    var remarks = await this.classBookStudentPrintRepository.GetRemarksAsync(schoolYear, classBookId, personId, ct);
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);

                    return new Student_Book_V_XIIModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ClassExam, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ControlExam, ct),
                        this.GetGradesModel(SchoolTerm.TermOne, grades),
                        this.GetRemarksModel(SchoolTerm.TermOne, remarks),
                        this.GetGradesModel(SchoolTerm.TermTwo, grades),
                        this.GetRemarksModel(SchoolTerm.TermTwo, remarks),
                        this.GetFinalGradesModel(grades),
                        this.GetAbsencesModel(absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetGradeResultsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetGradeResultSessionsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetSanctionsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
                }
            case ClassBookType.Book_CDO:
                {
                    var absencesByDate = await this.classBookStudentPrintRepository.GetAbsencesByDateAsync(schoolYear, classBookId, personId, ct);
                    return new Student_Book_CDOModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        this.GetAbsencesCdoModel(schoolYear, absencesByDate),
                        await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
                }
            case ClassBookType.Book_DPLR:
                return new Student_Book_DPLRModel(
                    this.GetCoverPageModel(schoolYear, coverPageData),
                    await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
            case ClassBookType.Book_CSOP:
                {
                    var grades = await this.classBookStudentPrintRepository.GetGradesAsync(schoolYear, classBookId, personId, ct);
                    var absencesByDate = await this.classBookStudentPrintRepository.GetAbsencesByDateAsync(schoolYear, classBookId, personId, ct);
                    var absences = await this.classBookStudentPrintRepository.GetAbsencesAsync(schoolYear, classBookId, personId, ct);
                    return new Student_Book_CSOPModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        this.GetGradesModel(SchoolTerm.TermOne, grades),
                        this.GetGradesModel(SchoolTerm.TermTwo, grades),
                        this.GetFinalGradesModel(grades),
                        coverPageData.BasicClassId == FirstClassBasicClassId
                            ? await this.GetFirstGradeResultsModelAsync(schoolYear, classBookId, personId, ct)
                            : new StudentFirstGradeResultsModel(null, null),
                        this.GetAbsencesCdoModel(schoolYear, absencesByDate),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, personId, ct),
                        this.GetAbsencesModel(absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, personId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, personId, ct));
                }
            default:
                throw new DomainException("Unknown BookType");
        }
    }

    private StudentCoverPageModel GetCoverPageModel(int schoolYear, GetStudentCoverPageDataVO coverPageData)
        => new(
            coverPageData.StudentName,
            schoolYear,
            coverPageData.InstitutionAbbreviation,
            coverPageData.InstitutionTownTypeName,
            coverPageData.InstitutionTownName,
            coverPageData.InstitutionMunicipalityName,
            coverPageData.InstitutionLocalAreaName,
            coverPageData.InstitutionRegionName,
            coverPageData.ClassName,
            coverPageData.SupportTypeName,
            coverPageData.ClassSpecialityName);

    private async Task<StudentTeachersModel> GetTeachersModelAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var teacherSubjects =
            await this.classBookStudentPrintRepository.GetTeacherSubjectsAsync(
                schoolYear,
                classBookId,
                classId,
                classIsLvl2,
                ct);

        return new(
            teacherSubjects
            .Select(
                ts => new StudentTeachersModelSubject(
                    ts.SubjectName,
                    ts.SubjectTypeName,
                    ts.TeacherName))
            .ToArray());
    }

    private async Task<StudentSchedulesModel> GetSchedulesModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var schedulesData =
            await this.classBookStudentPrintRepository.GetSchedulesDataAsync(
                schoolYear,
                classBookId,
                ct);

        var schedulesLessons =
            await this.classBookStudentPrintRepository.GetSchedulesLessonsAsync(
                schoolYear,
                classBookId,
                ct);

        var schedules =
            from sd in schedulesData
            join sl in schedulesLessons
            on sd.ScheduleId equals sl.ScheduleId
            group sl by new { sd.ScheduleId, sd.StartDate, sd.EndDate } into g
            select new StudentSchedulesModelSchedule(
                g.Key.StartDate,
                g.Key.EndDate,
                g.Select(
                    sl => new StudentSchedulesModelScheduleLessons(
                        sl.Day,
                        sl.HourNumber,
                        sl.SubjectName,
                        sl.SubjectTypeName)
                ).ToArray());

        return new(schedules.ToArray());
    }

    private async Task<StudentParentMeetingsModel> GetParentMeetingsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var parentMeetings =
            await this.classBookStudentPrintRepository.GetParentMeetingsAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            parentMeetings
            .Select(
                pm => new StudentParentMeetingsModelParentMeeting(
                    pm.Date,
                    pm.Title,
                    pm.Description))
            .ToArray());
    }

    private async Task<StudentExamsModel> GetExamsModelAsync(int schoolYear, int classBookId, BookExamType type, CancellationToken ct)
    {
        var exams =
            await this.classBookStudentPrintRepository.GetExamsAsync(
                schoolYear,
                classBookId,
                type,
                ct);

        return new(
            type,
            exams
            .Select(
                e => new StudentExamsModelExam(
                    e.SubjectName,
                    e.SubjectTypeName,
                    e.FirstTermDates,
                    e.SecondTermDates))
            .ToArray());
    }

    private StudentGradesModel GetGradesModel(SchoolTerm term, GetStudentGradesVO[] grades)
    {
        return new(
            term,
            grades
                .Where(g => g.Term == term && g.Type != GradeType.Final && g.Type != GradeType.Term)
                .Select(g => new StudentGradesModelGrade(
                    g.GradeDate,
                    g.CurriculumName,
                    this.GetGradeString(g.Category, g.DecimalGrade, g.SpecialGrade, g.QualitativeGrade, g.SubjectTypeId))
            ).ToArray()
        );
    }

    private StudentRemarksModel GetRemarksModel(SchoolTerm term, GetStudentRemarksVO[] remarks)
    {
        return new(
            term,
            remarks
                .Where(r => r.Term == term)
                .Select(g => new StudentRemarksModelRemark(
                    g.Date,
                    g.SubjectName,
                    g.SubjectTypeName,
                    g.Description)
            ).ToArray()
        );
    }

    private StudentAbsencesModel GetAbsencesModel(GetStudentAbsencesVO? absences)
    {
        if (absences != null)
        {
            return new StudentAbsencesModel(
                absences.FirstTermExcusedCount + absences.FirstTermCarriedExcusedCount,
                absences.FirstTermUnexcusedCount + absences.FirstTermLateCount * 0.5M + absences.FirstTermCarriedUnexcusedCount + absences.FirstTermCarriedLateCount * 0.5M,
                absences.SecondTermExcusedCount + absences.SecondTermCarriedExcusedCount,
                absences.SecondTermUnexcusedCount + absences.SecondTermLateCount * 0.5M + absences.SecondTermCarriedUnexcusedCount + absences.SecondTermCarriedLateCount * 0.5M,
                absences.WholeYearExcusedCount + absences.WholeYearCarriedExcusedCount,
                absences.WholeYearUnexcusedCount + absences.WholeYearLateCount * 0.5M + absences.WholeYearCarriedUnexcusedCount + absences.WholeYearCarriedLateCount * 0.5M
            );
        }
        else
        {
            return new StudentAbsencesModel(
                0,
                0,
                0,
                0,
                0,
                0
            );
        }
    }

    private StudentFinalGradesModel GetFinalGradesModel(GetStudentGradesVO[] grades)
    {
        var grouped = grades
        .GroupBy(g => new
        {
            g.CurriculumId,
            g.CurriculumPartID,
            g.IsIndividualLesson,
            g.TotalTermHours,
            g.SubjectTypeId,
            g.CurriculumName,
            g.ParentCurriculumId,
            g.CurriculumIsValid
        })
        .Select(g1 =>
        {
            var currentGrades = grades.Where(g => g.CurriculumId == g1.Key.CurriculumId);

            return new
            {
                g1.Key.CurriculumId,
                g1.Key.ParentCurriculumId,
                g1.Key.CurriculumPartID,
                g1.Key.IsIndividualLesson,
                g1.Key.TotalTermHours,
                Item = new StudentFinalGradesModelItem(
                    g1.Key.CurriculumName + (g1.Key.CurriculumIsValid ? "" : " (АРХИВИРАН)"),
                    currentGrades.Where(g => g.Type == GradeType.Term && g.Term == SchoolTerm.TermOne).Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, g1.Key.SubjectTypeId)).FirstOrDefault(),
                    currentGrades.Where(g => g.Type == GradeType.Term && g.Term == SchoolTerm.TermTwo).Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, g1.Key.SubjectTypeId)).FirstOrDefault(),
                    currentGrades.Where(g => g.Type == GradeType.Final).Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, g1.Key.SubjectTypeId)).FirstOrDefault()
                )
            };
        })
        .ToList();

        var ordered = grouped
            .Where(x => x.ParentCurriculumId == null)
            .OrderBy(x => x.CurriculumPartID)
            .ThenBy(x => x.IsIndividualLesson)
            .ThenByDescending(x => x.TotalTermHours)
            .SelectMany(parent =>
                new[] { parent }
                .Concat(
                    grouped
                        .Where(x => x.ParentCurriculumId == parent.CurriculumId)
                        .OrderBy(x => x.CurriculumPartID)
                        .ThenBy(x => x.IsIndividualLesson)
                        .ThenByDescending(x => x.TotalTermHours)
                )
            )
            .Select(x => x.Item)
            .ToArray();

        return new StudentFinalGradesModel(ordered);
    }

    private async Task<StudentFirstGradeResultsModel> GetFirstGradeResultsModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var firstGradeResults =
            await this.classBookStudentPrintRepository.GetFirstGradeResultAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(firstGradeResults.QualitativeGrade, firstGradeResults.SpecialGrade);
    }

    private async Task<StudentGradeResultsModel> GetGradeResultsModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var gradeResults =
            await this.classBookStudentPrintRepository.GetGradeResultsAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        if (gradeResults == null)
        {
            return new(null);
        }

        return new(gradeResults.InitialResult == GradeResultType.CompletesGrade ? "Завършва" : gradeResults.InitialResult == GradeResultType.MustRetakeExams ? $"Полага изпит/и по: {string.Join(", ", gradeResults.RetakeSubjectNames)}" : null);
    }

    private async Task<StudentGradeResultSessionsModel> GetGradeResultSessionsModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var gradeResultSessions =
            await this.classBookStudentPrintRepository.GetGradeResultSessionsAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(
            gradeResultSessions.Sessions.Select(
                s => new StudentGradeResultSessionsModelSession(
                    s.SubjectName,
                    s.Session1NoShow == true ? "Неявил се" : GradeUtils.GetDecimalGradeText(s.Session1Grade),
                    s.Session2NoShow == true ? "Неявил се" : GradeUtils.GetDecimalGradeText(s.Session2Grade),
                    s.Session3NoShow == true ? "Неявил се" : GradeUtils.GetDecimalGradeText(s.Session3Grade))
            ).ToArray(),
            gradeResultSessions.FinalResult.GetEnumDescription());
    }

    private async Task<StudentSanctionsModel> GetSanctionsModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var sanctions =
            await this.classBookStudentPrintRepository.GetSanctionsAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(
            sanctions.Select(
                s => new StudentSanctionsModelSanction(
                    s.SanctionType,
                    s.OrderNumber,
                    s.OrderDate,
                    s.CancelOrderNumber,
                    s.CancelOrderDate)
            ).ToArray());
    }

    private async Task<StudentSupportsModel> GetSupportsModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var supports =
            await this.classBookStudentPrintRepository.GetSupportsAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(
            supports
            .Select(
                s => new StudentSupportsModelSupport(
                    s.Difficulties,
                    s.Description,
                    s.ExpectedResult,
                    s.EndDate,
                    s.Teachers,
                    s.Activities.Select(
                        a => new StudentSupportsModelActivity(
                            a.ActivityType,
                            a.Date,
                            a.Target,
                            a.Result)
                    ).ToArray()))
            .ToArray());
    }

    private async Task<StudentNotesModel> GetNotesModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var notes =
            await this.classBookStudentPrintRepository.GetNotesAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(
            notes
            .Select(
                n => new StudentNotesModelNote(
                    n.DateCreated,
                    n.CreatedBySysUserName,
                    n.Description))
            .ToArray());
    }

    private async Task<StudentAttendancesModel> GetAttendancesModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var schoolYearLimitDates = await this.schedulesQueryRepository.GetSchoolYearSettingsAsync(schoolYear, classBookId, ct);
        var offDayDatesPg = await this.schedulesQueryRepository.GetOffDatesPgAsync(schoolYear, classBookId, ct);
        var attendances = await this.classBookStudentPrintRepository.GetAttendancesAsync(schoolYear, classBookId, personId, ct);

        var months = new List<StudentAttendancesModelMonth>();

        foreach (var month in DateExtensions.GetMonthsInRange(schoolYearLimitDates.SchoolYearStartDate, schoolYearLimitDates.SchoolYearEndDate))
        {
            var monthStart = new DateTime(month.year, month.month, 1);
            var monthEnd = new DateTime(month.year, month.month, 1).AddMonths(1).AddDays(-1);
            var monthText = monthStart.ToString("MMMM");

            var days = DateExtensions.GetDatesInRange(monthStart, monthEnd).Select(date =>
            {
                var isOutsideSchoolYearLimits = !(schoolYearLimitDates.SchoolYearStartDate <= date && date <= schoolYearLimitDates.SchoolYearEndDate);
                var existsOffDay = offDayDatesPg.Any(od => date == od.Date && !od.IsPgOffProgramDay);
                var isOffDay =
                    date.DayOfWeek == DayOfWeek.Saturday ||
                    date.DayOfWeek == DayOfWeek.Sunday ||
                    existsOffDay;

                return new
                {
                    Date = date,
                    IsOffDay = isOffDay,
                    IsWithoutTotal = isOffDay || isOutsideSchoolYearLimits
                };
            })
            .ToArray();

            var studentAttendances = new AttendanceType?[days.Length];
            var studentPresencesTotal = 0;

            foreach (var attendance in attendances.Where(a => a.Date.Month == month.month))
            {
                var day = attendance.Date.Day;

                studentAttendances[day - 1] = attendance.Type;

                if (attendance.Type == AttendanceType.Presence)
                {
                    studentPresencesTotal++;
                }
            }

            months.Add(
                new StudentAttendancesModelMonth(
                    monthText,
                    days.Select(d => new StudentAttendancesModelDay(d.Date.Day, d.IsOffDay, d.IsWithoutTotal)).ToArray(),
                    new StudentAttendancesModelAttendance(studentAttendances, studentPresencesTotal)
                )
            );
        }

        return new(months.ToArray());
    }

    private async Task<StudentPgResultsModel> GetPgResultsModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var pgResults =
            await this.classBookStudentPrintRepository.GetPgResultsAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(
            pgResults
            .Select(
                r => new StudentPgResultsModelPgResult(
                    r.StartSchoolYearResult,
                    r.EndSchoolYearResult))
            .ToArray());
    }

    private StudentAbsencesCdoModel GetAbsencesCdoModel(int schoolYear, GetStudentAbsencesByDateVO[] absencesByDate)
    {
        var oct1st = new DateTime(schoolYear, 10, 1);
        var nov1st = new DateTime(schoolYear, 11, 1);
        var dec1st = new DateTime(schoolYear, 12, 1);
        var jan1st = new DateTime(schoolYear + 1, 1, 1);
        var feb1st = new DateTime(schoolYear + 1, 2, 1);
        var mar1st = new DateTime(schoolYear + 1, 3, 1);
        var apr1st = new DateTime(schoolYear + 1, 4, 1);
        var may1st = new DateTime(schoolYear + 1, 5, 1);
        var jun1st = new DateTime(schoolYear + 1, 6, 1);
        var jul1st = new DateTime(schoolYear + 1, 7, 1);
        var aug1st = new DateTime(schoolYear + 1, 8, 1);

        Func<IEnumerable<GetStudentAbsencesByDateVO>, StudentAbsencesItemByTypes> aggregate =
            (absences) =>
            {
                var e = absences.Sum(gi => gi.ExcusedCount);
                var u = absences.Sum(gi => gi.UnexcusedCount);
                var l = absences.Sum(gi => gi.LateCount);
                return new StudentAbsencesItemByTypes(e, u + l * 0.5M);
            };

        return new(
            aggregate(absencesByDate.Where(a => a.Date < oct1st)),
            aggregate(absencesByDate.Where(a => oct1st <= a.Date && a.Date < nov1st)),
            aggregate(absencesByDate.Where(a => nov1st <= a.Date && a.Date < dec1st)),
            aggregate(absencesByDate.Where(a => dec1st <= a.Date && a.Date < jan1st)),
            aggregate(absencesByDate.Where(a => jan1st <= a.Date && a.Date < feb1st)),
            aggregate(absencesByDate.Where(a => feb1st <= a.Date && a.Date < mar1st)),
            aggregate(absencesByDate.Where(a => mar1st <= a.Date && a.Date < apr1st)),
            aggregate(absencesByDate.Where(a => apr1st <= a.Date && a.Date < may1st)),
            aggregate(absencesByDate.Where(a => may1st <= a.Date && a.Date < jun1st)),
            aggregate(absencesByDate.Where(a => jun1st <= a.Date && a.Date < jul1st)),
            aggregate(absencesByDate.Where(a => jul1st <= a.Date && a.Date < aug1st)),
            aggregate(absencesByDate.Where(a => a.Date >= aug1st))
        );
    }

    private async Task<StudentIndividualWorksModel> GetIndividualWorksModelAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var individualWorks =
            await this.classBookStudentPrintRepository.GetIndividualWorksAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        return new(
            individualWorks
            .Select(
                iw => new StudentIndividualWorksModelIndividualWork(
                    iw.Date,
                    iw.IndividualWorkActivity))
            .ToArray());
    }

    private string GetGradeString(GradeCategory category, decimal? decimalGrade, SpecialNeedsGrade? specialGrade, QualitativeGrade? qualitativeGrade, int subjectTypeId)
    {
        var isProfilingSubjectGrade = Grade.SubjectTypeIsProfilingSubject(subjectTypeId);

        return category switch
        {
            GradeCategory.Decimal => decimalGrade switch
            {
                var gv when gv < 3.0m => isProfilingSubjectGrade ? $"{decimalGrade:0.00}" : "2",
                var gv when gv < 3.5m => isProfilingSubjectGrade ? $"{decimalGrade:0.00}" : "3",
                var gv when gv < 4.5m => isProfilingSubjectGrade ? $"{decimalGrade:0.00}" : "4",
                var gv when gv < 5.5m => isProfilingSubjectGrade ? $"{decimalGrade:0.00}" : "5",
                var gv when gv >= 5.5m => isProfilingSubjectGrade ? $"{decimalGrade:0.00}" : "6",
                _ => string.Empty
            },
            GradeCategory.SpecialNeeds => specialGrade switch
            {
                SpecialNeedsGrade.HasDificulty => "СЗ",
                SpecialNeedsGrade.DoingOk => "СС",
                SpecialNeedsGrade.MeetsExpectations => "ПИ",
                _ => string.Empty,
            },
            GradeCategory.Qualitative => qualitativeGrade switch
            {
                QualitativeGrade.Poor => "2",
                QualitativeGrade.Fair => "3",
                QualitativeGrade.Good => "4",
                QualitativeGrade.VeryGood => "5",
                QualitativeGrade.Excellent => "6",
                _ => string.Empty,
            },
            _ => string.Empty,
        };
    }
}
