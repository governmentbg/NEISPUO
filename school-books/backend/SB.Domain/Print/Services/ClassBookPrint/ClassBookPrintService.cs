namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;
using static SB.Domain.IClassBookPrintRepository;

internal class ClassBookPrintService : IPrintService
{
    private readonly IClassBookPrintRepository classBookPrintRepository;
    private readonly ISchedulesQueryRepository schedulesQueryRepository;
    private readonly IHtmlTemplateService htmlTemplateService;
    private readonly IServiceScopeFactory serviceScopeFactory;

    private readonly HashSet<GradeType> termGradeTypes = new() { GradeType.Term, GradeType.OtherClassTerm, GradeType.OtherSchoolTerm };

    public ClassBookPrintService(
        IClassBookPrintRepository classBookPrintRepository,
        ISchedulesQueryRepository schedulesQueryRepository,
        IHtmlTemplateService htmlTemplateService,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.classBookPrintRepository = classBookPrintRepository;
        this.schedulesQueryRepository = schedulesQueryRepository;
        this.htmlTemplateService = htmlTemplateService;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task RenderHtmlAsync(string printParamsStr, TextWriter textWriter, CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookPrintParams>(printParamsStr);
        var coverPageData =
            await this.classBookPrintRepository.GetCoverPageDataAsync(
                printParams!.SchoolYear,
                printParams.ClassBookId,
                ct);

        string template = coverPageData.BookType switch
        {
            ClassBookType.Book_PG => HtmlTemplateConfig.ClassBookPrint_Book_PG.TemplateName,
            ClassBookType.Book_I_III => HtmlTemplateConfig.ClassBookPrint_Book_I_III.TemplateName,
            ClassBookType.Book_IV => HtmlTemplateConfig.ClassBookPrint_Book_IV.TemplateName,
            ClassBookType.Book_V_XII => HtmlTemplateConfig.ClassBookPrint_Book_V_XII.TemplateName,
            ClassBookType.Book_CDO => HtmlTemplateConfig.ClassBookPrint_Book_CDO.TemplateName,
            ClassBookType.Book_DPLR => HtmlTemplateConfig.ClassBookPrint_Book_DPLR.TemplateName,
            ClassBookType.Book_CSOP => HtmlTemplateConfig.ClassBookPrint_Book_CSOP.TemplateName,
            _ => throw new DomainException("Unknown BookType"),
        };

        object model = await this.GetModelAsync(
            printParams.SchoolYear,
            printParams.ClassBookId,
            coverPageData,
            ct);

        await this.htmlTemplateService.RenderAsync(template, model, textWriter, ct);
    }

    public async Task FinalizePrintAsProcessedAsync(string printParamsStr, int printId, int blobId, string? contentHash, CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookPrintParams>(printParamsStr);
        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var classBooksAggregateRepository = scope.ServiceProvider.GetRequiredService<IClassBooksAggregateRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var classBook = await classBooksAggregateRepository.FindAsync(printParams!.SchoolYear, printParams.ClassBookId, ct);

        classBook.ClassBookPrintMarkProcessed(printId, blobId, contentHash);

        await unitOfWork.SaveAsync(ct);
    }

    public async Task FinalizePrintAsErroredAsync(string printParamsStr, int printId, CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookPrintParams>(printParamsStr);
        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var classBooksAggregateRepository = scope.ServiceProvider.GetRequiredService<IClassBooksAggregateRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var classBook = await classBooksAggregateRepository.FindAsync(printParams!.SchoolYear, printParams.ClassBookId, ct);

        classBook.ClassBookPrintMarkErrored(printId);

        await unitOfWork.SaveAsync(ct);
    }

    private async Task<object> GetModelAsync(int schoolYear, int classBookId, GetCoverPageDataVO coverPageData, CancellationToken ct)
    {
        (int instId, int classId, bool classIsLvl2) = await this.classBookPrintRepository.GetClassBookInfoAsync(schoolYear, classBookId, ct);
        var students =
            await this.classBookPrintRepository.GetStudentsDataAsync(
                schoolYear,
                instId,
                classBookId,
                classId,
                classIsLvl2,
                ct);
        var removedStudents =
                await this.classBookPrintRepository.GetRemovedStudentsDataAsync(
                    schoolYear,
                    classBookId,
                    students.Select(s => s.PersonId).ToArray(),
                    ct);
        students = students.Concat(removedStudents).ToArray();

        switch (coverPageData.BookType)
        {
            case ClassBookType.Book_PG:
                return new Book_PGModel(
                    this.GetCoverPageModel(schoolYear, coverPageData),
                    this.GetStudentsModel(true, students),
                    await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                    await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                    await this.GetAttendancesModelAsync(schoolYear, classBookId, students, ct),
                    await this.GetPgResultsModelAsync(schoolYear, classBookId, ct),
                    await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, false, ct),
                    await this.GetNotesModelAsync(schoolYear, classBookId, ct));
            case ClassBookType.Book_I_III:
                {
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);
                    var grades = await this.classBookPrintRepository.GetGradesAsync(schoolYear, classBookId, ct);
                    var absences = await this.classBookPrintRepository.GetAbsencesAsync(schoolYear, classBookId, ct);
                    return new Book_I_IIIModel(
                        coverPageData.BasicClassId != ClassBook.FirstGradeBasicClassId,
                        coverPageData.BasicClassId != ClassBook.SecondGradeBasicClassId && coverPageData.BasicClassId != ClassBook.ThirdGradeBasicClassId,
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ClassExam, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ControlExam, ct),
                        this.GetStudentsModel(false, students),
                        await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, false, ct),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermOne, students, curriculum, grades),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermTwo, students, curriculum, grades),
                        await this.GetRemarksModelAsync(schoolYear, classBookId, ct),
                        this.GetFinalGradesModel(students, curriculum, grades),
                        coverPageData.BasicClassId != ClassBook.SecondGradeBasicClassId && coverPageData.BasicClassId != ClassBook.ThirdGradeBasicClassId
                            ? await this.GetFirstGradeResultsModelAsync(schoolYear, classBookId, ct)
                            : new FirstGradeResultsModel(Array.Empty<FirstGradeResultsModelFirstGradeResult>()),
                        this.GetAbsencesModel(students, absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, ct),
                        await this.GetSanctionsModelAsync(schoolYear, classBookId, ct),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, ct));
                }
            case ClassBookType.Book_IV:
                {
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);
                    var grades = await this.classBookPrintRepository.GetGradesAsync(schoolYear, classBookId, ct);
                    var absences = await this.classBookPrintRepository.GetAbsencesAsync(schoolYear, classBookId, ct);
                    return new Book_IVModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ClassExam, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ControlExam, ct),
                        this.GetStudentsModel(false, students),
                        await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, false, ct),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermOne, students, curriculum, grades),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermTwo, students, curriculum, grades),
                        await this.GetRemarksModelAsync(schoolYear, classBookId, ct),
                        this.GetFinalGradesModel(students, curriculum, grades),
                        this.GetAbsencesModel(students, absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, ct),
                        await this.GetSanctionsModelAsync(schoolYear, classBookId, ct),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, ct));
                }
            case ClassBookType.Book_V_XII:
                {
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);
                    var grades = await this.classBookPrintRepository.GetGradesAsync(schoolYear, classBookId, ct);
                    var absences = await this.classBookPrintRepository.GetAbsencesAsync(schoolYear, classBookId, ct);
                    return new Book_V_XIIModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ClassExam, ct),
                        await this.GetExamsModelAsync(schoolYear, classBookId, BookExamType.ControlExam, ct),
                        this.GetStudentsModel(false, students),
                        await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, false, ct),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermOne, students, curriculum, grades),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermTwo, students, curriculum, grades),
                        await this.GetRemarksModelAsync(schoolYear, classBookId, ct),
                        this.GetFinalGradesModel(students, curriculum, grades),
                        this.GetAbsencesModel(students, absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, ct),
                        await this.GetGradeResultsModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetGradeResultSessionsModelAsync(schoolYear, classBookId, ct),
                        await this.GetSanctionsModelAsync(schoolYear, classBookId, ct),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, ct));
                }
            case ClassBookType.Book_CDO:
                {
                    var absencesByDate = await this.classBookPrintRepository.GetAbsencesByDateAsync(schoolYear, classBookId, ct);
                    var teachers = await this.classBookPrintRepository.GetTeacherInfoAndSubjectsAsync(schoolYear, classBookId, classId, classIsLvl2, ct);
                    var teachersInfo =
                        string.Join(", ", teachers
                            .SelectMany(t => t.Teachers)
                            .Select(t => t.GetNamesAndPhone())
                            .Distinct());

                    return new Book_CDOModel(
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        this.GetStudentsModel(false, students, teachersInfo),
                        this.GetAbsencesCdoModel(schoolYear, students, absencesByDate),
                        new SchoolYearProgramModel(coverPageData.SchoolYearProgram),
                        await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, false, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, ct));
                }
            case ClassBookType.Book_DPLR:
                {
                    var teachers = await this.classBookPrintRepository.GetTeacherInfoAndSubjectsAsync(schoolYear, classBookId, classId, classIsLvl2, ct);
                    var teachersInfo =
                        string.Join(", ", teachers
                            .SelectMany(t => t.Teachers)
                            .Select(t => t.GetNamesAndPhone())
                            .Distinct());

                    return new Book_DPLRModel(
                    this.GetCoverPageModel(schoolYear, coverPageData),
                    this.GetStudentsModel(false, students, teachersInfo),
                    await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, true, ct),
                    await this.GetPerformancesModelAsync(schoolYear, classBookId, ct),
                    await this.GetReplrParticipationsModelAsync(schoolYear, classBookId, ct),
                    await this.GetNotesModelAsync(schoolYear, classBookId, ct));
                }
            case ClassBookType.Book_CSOP:
                {
                    var curriculum =
                        await this.classBookPrintRepository.GetCurriculumAsync(
                            schoolYear,
                            classBookId,
                            classId,
                            classIsLvl2,
                            ct);
                    var grades = await this.classBookPrintRepository.GetGradesAsync(schoolYear, classBookId, ct);
                    var absencesByDate = await this.classBookPrintRepository.GetAbsencesByDateAsync(schoolYear, classBookId, ct);
                    var absences = await this.classBookPrintRepository.GetAbsencesAsync(schoolYear, classBookId, ct);
                    return new Book_CSOPModel(
                        coverPageData.BasicClassId != ClassBook.FirstGradeBasicClassId,
                        coverPageData.BasicClassId != ClassBook.SecondGradeBasicClassId && coverPageData.BasicClassId != ClassBook.ThirdGradeBasicClassId,
                        this.GetCoverPageModel(schoolYear, coverPageData),
                        await this.GetTeachersModelAsync(schoolYear, classBookId, classId, classIsLvl2, ct),
                        await this.GetSchedulesModelAsync(schoolYear, classBookId, ct),
                        await this.GetParentMeetingsModelAsync(schoolYear, classBookId, ct),
                        this.GetStudentsModel(false, students),
                        this.GetAbsencesCdoModel(schoolYear, students, absencesByDate),
                        await this.GetScheduleAndAbsencesModelAsync(schoolYear, classBookId, false, ct),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermOne, students, curriculum, grades),
                        this.GetGradesModel(schoolYear, SchoolTerm.TermTwo, students, curriculum, grades),
                        this.GetFinalGradesModel(students, curriculum, grades),
                        coverPageData.BasicClassId == ClassBook.FirstGradeBasicClassId
                        ? await this.GetFirstGradeResultsModelAsync(schoolYear, classBookId, ct)
                        : new FirstGradeResultsModel(Array.Empty<FirstGradeResultsModelFirstGradeResult>()),
                        await this.GetSupportsModelAsync(schoolYear, classBookId, ct),
                        this.GetAbsencesModel(students, absences),
                        await this.GetIndividualWorksModelAsync(schoolYear, classBookId, ct),
                        await this.GetNotesModelAsync(schoolYear, classBookId, ct));
                }
            default:
                throw new DomainException("Unknown BookType");
        }
    }

    private CoverPageModel GetCoverPageModel(int schoolYear, GetCoverPageDataVO coverPageData)
        => new(
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

    private async Task<TeachersModel> GetTeachersModelAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var teacherSubjects =
            await this.classBookPrintRepository.GetTeacherInfoAndSubjectsAsync(
                schoolYear,
                classBookId,
                classId,
                classIsLvl2,
                ct);

        return new(
            teacherSubjects
            .Distinct()
            .Select(
                ts => new TeachersModelSubject(
                    ts.SubjectName,
                    ts.SubjectTypeName,
                    string.Join(", ", ts.Teachers.Select(t => t.Names))))
            .ToArray());
    }

    private async Task<SchedulesModel> GetSchedulesModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var schedulesData =
            await this.classBookPrintRepository.GetSchedulesDataAsync(
                schoolYear,
                classBookId,
                ct);

        var schedulesLessons =
            await this.classBookPrintRepository.GetSchedulesLessonsAsync(
                schoolYear,
                classBookId,
                ct);

        var schedules =
            from sd in schedulesData
            join sl in schedulesLessons
            on sd.ScheduleId equals sl.ScheduleId
            group sl by new { sd.ScheduleId, sd.StudentPersonId, sd.StudentName, sd.StartDate, sd.EndDate } into g
            select new SchedulesModelSchedule(
                g.Key.StudentPersonId,
                (string?)g.Key.StudentName,
                g.Key.StartDate,
                g.Key.EndDate,
                g.Select(
                    sl => new SchedulesModelScheduleLessons(
                        sl.Day,
                        sl.HourNumber,
                        sl.SubjectName,
                        sl.SubjectTypeName)
                ).ToArray());

        return new(
            schedules.Where(s => s.StudentPersonId == null).ToArray(),
            schedules.Where(s => s.StudentPersonId != null).ToArray());
    }

    private async Task<ParentMeetingsModel> GetParentMeetingsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var parentMeetings =
            await this.classBookPrintRepository.GetParentMeetingsAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            parentMeetings
            .Select(
                pm => new ParentMeetingsModelParentMeeting(
                    pm.Date,
                    pm.Title,
                    pm.Description))
            .ToArray());
    }

    private async Task<ExamsModel> GetExamsModelAsync(int schoolYear, int classBookId, BookExamType type, CancellationToken ct)
    {
        var exams =
            await this.classBookPrintRepository.GetExamsAsync(
                schoolYear,
                classBookId,
                type,
                ct);

        return new(
            type,
            exams
            .Select(
                e => new ExamsModelExam(
                    e.SubjectName,
                    e.SubjectTypeName,
                    e.FirstTermDates,
                    e.SecondTermDates))
            .ToArray());
    }

    private StudentsModel GetStudentsModel(bool isPG, GetStudentsDataVO[] students, string? teachersInfo = null)
    {
        static string? commaJoin(string?[] strings) => strings != null ? string.Join(
                    ", ",
                    strings.Where(s => !string.IsNullOrWhiteSpace(s))) : null;

        return new(
            isPG,
            students
            .Select(
                s => new StudentsModelStudent(
                    s.ClassNumber,
                    StringUtils.JoinNames(s.FirstName, s.MiddleName, s.LastName),
                    s.BirthDate,
                    s.PersonalId,
                    s.IsForeignBorn ? commaJoin(new[] { s.BirthCountryName, s.BirthTownName }) : $"{s.BirthTownType} {s.BirthTownName}",
                    commaJoin(new[] { $"{s.AddressTownType} {s.AddressTownName}", s.AddressText }),
                    commaJoin(new[] { s.DoctorName, s.DoctorPhone }),
                    s.EnrollmentDate,
                    s.DischargeDate,
                    s.EnrolledClassName,
                    s.Profession,
                    s.Relatives.Select(r => StringUtils.JoinNames(r.FirstName, r.LastName) + (!string.IsNullOrWhiteSpace(r.PhoneNumber) ? $" - {r.PhoneNumber}" : "")).ToArray(),
                    s.RelocationDocumentNoteNumber,
                    s.RelocationDocumentNoteDate,
                    s.AdmissionDocumentNoteNumber,
                    s.AdmissionDocumentNoteDate,
                    s.AdmissionReasonType,
                    s.IsTransferred,
                    teachersInfo
                ))
            .ToArray());
    }

    private GradesModel GetGradesModel(int schoolYear, SchoolTerm term, GetStudentsDataVO[] students, GetCurriculumInfoVO[] curriculum, GetGradesVO[] grades)
    {
        var oct1st = new DateTime(schoolYear, 10, 1);
        var nov1st = new DateTime(schoolYear, 11, 1);
        var dec1st = new DateTime(schoolYear, 12, 1);
        var jan1st = new DateTime(schoolYear + 1, 1, 1);
        var mar1st = new DateTime(schoolYear + 1, 3, 1);
        var apr1st = new DateTime(schoolYear + 1, 4, 1);
        var may1st = new DateTime(schoolYear + 1, 5, 1);
        var jun1st = new DateTime(schoolYear + 1, 6, 1);

        string[] formatGrades(IEnumerable<GetGradesVO> grades) => grades
                .OrderBy(i => i.GradeDate)
                .Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, i.SubjectTypeId))
                .ToArray();

        return new(
            term,
            curriculum
            .Select(
                e => new GradesModelCurriculumInfo(
                    e.CurriculumId,
                    e.SubjectName,
                    e.SubjectTypeName))
            .ToArray(),
            students
            .OrderBy(s => s.ClassNumber ?? 99999)
            .ThenBy(s => s.FirstName)
            .ThenBy(s => s.MiddleName)
            .ThenBy(s => s.LastName)
            .Select(
                e => new GradesModelStudentInfo(
                    e.PersonId,
                    e.ClassNumber,
                    StringUtils.JoinNames(e.FirstName, e.MiddleName, e.LastName),
                    e.IsTransferred))
            .ToArray(),
            grades
            .Where(g => g.Term == term && g.Type != GradeType.Final)
            .GroupBy(g1 => g1.PersonId)
                .Select(g1 => new GradesModelItem(
                    g1.Key,
                    g1.GroupBy(g2 => g2.CurriculumId).Select(g2 => new GradesModelItemByCurriculum(
                        g2.Key,
                        new GradesModelItemByMonths(
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && g.GradeDate < oct1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && oct1st <= g.GradeDate && g.GradeDate < nov1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && nov1st <= g.GradeDate && g.GradeDate < dec1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && dec1st <= g.GradeDate && g.GradeDate < jan1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && g.GradeDate >= jan1st && g.Term == SchoolTerm.TermOne)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && g.GradeDate < mar1st && g.Term == SchoolTerm.TermTwo)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && mar1st <= g.GradeDate && g.GradeDate < apr1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && apr1st <= g.GradeDate && g.GradeDate < may1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && may1st <= g.GradeDate && g.GradeDate < jun1st)),
                            formatGrades(g2.Where(g => !this.termGradeTypes.Contains(g.Type) && g.GradeDate >= jun1st)),
                            formatGrades(g2.Where(g => this.termGradeTypes.Contains(g.Type))).ToArray()
                        )
                    )).ToArray()
           )).ToArray()
        );
    }

    private async Task<RemarksModel> GetRemarksModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var remarks =
            await this.classBookPrintRepository.GetRemarksAsync(
                schoolYear,
                classBookId,
                ct);

        var remarkStudents =
            from rs in remarks
            group rs by new { rs.ClassNumber, rs.FullName, rs.IsTransferred } into g
            select new RemarksModelStudent(
                g.Key.ClassNumber,
                g.Key.FullName,
                g.Key.IsTransferred,
                g.Select(
                    r => new RemarksModelRemark(
                        r.Date,
                        r.RemarkType,
                        r.SubjectName,
                        r.SubjectTypeName,
                        r.Description)
                ).OrderBy(r => r.Date).ToArray());

        return new(remarkStudents.ToArray());
    }

    private async Task<ScheduleAndAbsencesModel> GetScheduleAndAbsencesModelAsync(int schoolYear, int classBookId, bool isDPLR, CancellationToken ct)
    {
        var schedule =
            (await this.classBookPrintRepository.GetScheduleAndAbsencesByWeekAsync(
                schoolYear,
                classBookId,
                ct))
            .Select(
                s => new ScheduleAndAbsencesModelWeek(
                    s.WeekName,
                    s.StudentName,
                    s.AdditionalActivities,
                    s.Days.Select(
                        d => new ScheduleAndAbsencesModelWeekDay(
                            d.Date,
                            d.DayName,
                            d.IsOffDay,
                            d.Hours.Select(
                                h => new ScheduleAndAbsencesModelWeekDayHour(
                                    h.HourNumber,
                                    h.IsEmptyHour,
                                    h.ReplTeacherIsNonSpecialist,
                                    h.SubjectName,
                                    h.SubjectTypeName,
                                    string.IsNullOrEmpty(h.ReplTeacher) && string.IsNullOrEmpty(h.ExtReplTeacher) ? string.Join(", ", h.CurriculumTeachers) :
                                        !string.IsNullOrEmpty(h.ReplTeacher) ? h.ReplTeacher + " (зам.)" : h.ExtReplTeacher + " (външен лектор)",
                                    string.Join(", ", h.ExcusedStudentClassNumbers),
                                    string.Join(", ", h.UnexcusedStudentClassNumbers),
                                    string.Join(", ", h.LateStudentClassNumbers),
                                    string.Join(", ", h.DplrAbsences),
                                    string.Join(", ", h.DplrAttendances),
                                    h.Topics,
                                    h.Location
                                )
                            ).ToArray()
                        )
                    ).ToArray()
                )
             );

        return new(
            isDPLR,
            schedule
            .Where(s => string.IsNullOrEmpty(s.StudentName))
            .ToArray(),
            schedule
            .Where(s => !string.IsNullOrEmpty(s.StudentName))
            .ToArray()
        );
    }

    private AbsencesModel GetAbsencesModel(GetStudentsDataVO[] students, GetAbsencesVO[] absences)
    {
        var absencesDict = absences.ToDictionary(a => a.PersonId);

        return new(
            students
            .OrderBy(s => s.ClassNumber ?? 99999)
            .ThenBy(s => s.FirstName)
            .ThenBy(s => s.MiddleName)
            .ThenBy(s => s.LastName)
            .Select(s =>
            {
                var absence = absencesDict.GetValueOrDefault(s.PersonId);

                return absence != null ?
                    new AbsencesModelAbsence(
                        s.ClassNumber,
                        StringUtils.JoinNames(s.FirstName, s.MiddleName, s.LastName),
                        s.IsTransferred,
                        absence.FirstTermExcusedCount + absence.FirstTermCarriedExcusedCount,
                        absence.FirstTermUnexcusedCount + absence.FirstTermLateCount * 0.5M + absence.FirstTermCarriedUnexcusedCount + absence.FirstTermCarriedLateCount * 0.5M,
                        absence.WholeYearExcusedCount + absence.WholeYearCarriedExcusedCount,
                        absence.WholeYearUnexcusedCount + absence.WholeYearLateCount * 0.5M + absence.WholeYearCarriedUnexcusedCount + absence.WholeYearCarriedLateCount * 0.5M)
                    : new AbsencesModelAbsence(
                        s.ClassNumber,
                        StringUtils.JoinNames(s.FirstName, s.MiddleName, s.LastName),
                        s.IsTransferred,
                        0,
                        0,
                        0,
                        0);
            })
            .ToArray());
    }

    private FinalGradesModel GetFinalGradesModel(GetStudentsDataVO[] students, GetCurriculumInfoVO[] curriculum, GetGradesVO[] grades)
    {
        return new(
            curriculum
            .Select(
                e => new FinalGradesModelCurriculumInfo(
                    e.CurriculumId,
                    e.SubjectName,
                    e.SubjectTypeName))
            .ToArray(),
            students
            .OrderBy(s => s.ClassNumber ?? 99999)
            .ThenBy(s => s.FirstName)
            .ThenBy(s => s.MiddleName)
            .ThenBy(s => s.LastName)
            .Select(
                e => new FinalGradesModelStudentInfo(
                    e.PersonId,
                    e.ClassNumber,
                    StringUtils.JoinNames(e.FirstName, e.MiddleName, e.LastName),
                    e.IsTransferred))
            .ToArray(),
            students
            .Select(student =>
                new FinalGradesModelItem(
                    student.PersonId,
                    curriculum.Select(curr =>
                    {
                        var gradelessCurr = student.GradelessCurriculums?.FirstOrDefault(gc => gc.Curriculum == curr.CurriculumId);

                        var gradesByCurriculum =
                            grades
                                .Where(g =>
                                    g.PersonId == student.PersonId &&
                                    g.CurriculumId == curr.CurriculumId);

                        string? firstTermGrade = gradelessCurr?.WithoutFirstTermGrade == true
                            ? "ОСВ."
                            : gradesByCurriculum
                                .Where(g => g.Type == GradeType.Term && g.Term == SchoolTerm.TermOne)
                                .Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, i.SubjectTypeId))
                                .FirstOrDefault();

                        string? secondTermGrade = gradelessCurr?.WithoutSecondTermGrade == true
                            ? "ОСВ."
                            : gradesByCurriculum
                                .Where(g => g.Type == GradeType.Term && g.Term == SchoolTerm.TermTwo)
                                .Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, i.SubjectTypeId))
                                .FirstOrDefault();

                        string? finalGrade = gradelessCurr?.WithoutFinalGrade == true
                            ? "ОСВ."
                            : gradesByCurriculum
                                .Where(g => g.Type == GradeType.Final)
                                .Select(i => this.GetGradeString(i.Category, i.DecimalGrade, i.SpecialGrade, i.QualitativeGrade, i.SubjectTypeId))
                                .FirstOrDefault();

                        return new FinalGradesModelItemByCurriculum(
                            curr.CurriculumId,
                            new FinalGradesModelItemByType(
                                firstTermGrade,
                                secondTermGrade,
                                finalGrade
                            )
                        );
                    }).ToArray()
                )
            ).ToArray()
        );
    }

    private async Task<FirstGradeResultsModel> GetFirstGradeResultsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var firstGradeResults =
            await this.classBookPrintRepository.GetFirstGradeResultsAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            firstGradeResults
            .Select(
                gr => new FirstGradeResultsModelFirstGradeResult(
                    gr.ClassNumber,
                    gr.FullName,
                    gr.IsTransferred,
                    gr.QualitativeGrade,
                    gr.SpecialGrade))
            .ToArray());
    }

    private async Task<GradeResultsModel> GetGradeResultsModelAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var gradeResults =
            await this.classBookPrintRepository.GetGradeResultsAsync(
                schoolYear,
                classBookId,
                classId,
                classIsLvl2,
                ct);

        return new(
            gradeResults
            .Select(
                gr => new GradeResultsModelResult(
                    gr.ClassNumber,
                    gr.FullName,
                    gr.IsTransferred,
                    gr.InitialResult == GradeResultType.CompletesGrade ? "Завършва" : gr.InitialResult == GradeResultType.MustRetakeExams ? $"Полага изпит/и по: {string.Join(", ", gr.RetakeSubjectNames)}" : null))
            .ToArray());
    }

    private async Task<GradeResultSessionsModel> GetGradeResultSessionsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var gradeResultSessions =
            await this.classBookPrintRepository.GetGradeResultSessionsAsync(
                schoolYear,
                classBookId,
                ct);

        var gradeResultStudents =
            from grs in gradeResultSessions
            group grs by new { grs.ClassNumber, grs.FullName, grs.IsTransferred, grs.FinalResultType } into g
            select new GradeResultSessionsModelStudent(
                g.Key.ClassNumber,
                g.Key.FullName,
                g.Key.IsTransferred,
                g.Select(
                    s => new GradeResultSessionsModelSession(
                        s.SubjectName,
                        s.Session1NoShow == true ? "Неявил се" : GradeUtils.GetDecimalGradeText(s.Session1Grade),
                        s.Session2NoShow == true ? "Неявил се" : GradeUtils.GetDecimalGradeText(s.Session2Grade),
                        s.Session3NoShow == true ? "Неявил се" : GradeUtils.GetDecimalGradeText(s.Session3Grade))
                ).ToArray(),
                g.Key.FinalResultType.GetEnumDescription());

        return new(gradeResultStudents.ToArray());
    }

    private async Task<SanctionsModel> GetSanctionsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var sanctions =
            await this.classBookPrintRepository.GetSanctionsAsync(
                schoolYear,
                classBookId,
                ct);

        var sanctionStudents =
            from grs in sanctions
            group grs by new { grs.ClassNumber, grs.FullName, grs.IsTransferred } into g
            select new SanctionsModelStudent(
                g.Key.ClassNumber,
                g.Key.FullName,
                g.Key.IsTransferred,
                g.Select(
                    s => new SanctionsModelSanction(
                        s.SanctionType,
                        s.OrderNumber,
                        s.OrderDate,
                        s.CancelOrderNumber,
                        s.CancelOrderDate)
                ).ToArray());

        return new(sanctionStudents.ToArray());
    }

    private async Task<SupportsModel> GetSupportsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var supports =
            await this.classBookPrintRepository.GetSupportsAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            supports
            .Select(
                s => new SupportsModelSupport(
                    s.Students,
                    s.Difficulties,
                    s.Description,
                    s.ExpectedResult,
                    s.EndDate,
                    s.Teachers,
                    s.Activities.Select(
                        a => new SupportsModelActivity(
                            a.ActivityType,
                            a.Date,
                            a.Target,
                            a.Result)
                    ).ToArray()))
            .ToArray());
    }

    private async Task<NotesModel> GetNotesModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var notes =
            await this.classBookPrintRepository.GetNotesAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            notes
            .Select(
                n => new NotesModelNote(
                    n.IsForAllStudents ? "Всички" : string.Join(", ", n.Students),
                    n.Description))
            .ToArray());
    }

    private async Task<AttendancesModel> GetAttendancesModelAsync(int schoolYear, int classBookId, GetStudentsDataVO[] students, CancellationToken ct)
    {
        var schoolYearLimitDates = await this.schedulesQueryRepository.GetSchoolYearSettingsAsync(schoolYear, classBookId, ct);
        var offDayDatesPg = await this.schedulesQueryRepository.GetOffDatesPgAsync(schoolYear, classBookId, ct);
        var attendances = await this.classBookPrintRepository.GetAttendancesAsync(schoolYear, classBookId, ct);

        var months = new List<AttendancesModelMonth>();

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

            var studentsInfo = students.ToDictionary(s => s.PersonId, s => (s.ClassNumber, FullName: StringUtils.JoinNames(s.FirstName, s.LastName), s.IsTransferred));

            var studentAttendances = students
                .ToDictionary(
                    s => s.PersonId,
                    s => new AttendanceType?[days.Length]);
            var studentPresencesTotal = students
                .ToDictionary(
                    s => s.PersonId,
                    s => 0);

            var presencesDayTotals = Enumerable.Repeat(0, days.Length).ToArray();
            var presencesCountTotal = 0;

            foreach (var attendance in attendances.Where(a => a.Date.Month == month.month))
            {
                var day = attendance.Date.Day;

                studentAttendances[attendance.PersonId][day - 1] = attendance.Type;

                if (attendance.Type == AttendanceType.Presence)
                {
                    studentPresencesTotal[attendance.PersonId]++;
                    presencesDayTotals[day - 1]++;
                    presencesCountTotal++;
                }
            }

            var averagePresencesCount = (double)presencesCountTotal / days.Count(d => !d.IsWithoutTotal);

            months.Add(
                new AttendancesModelMonth(
                    monthText,
                    days.Select(d => new AttendancesModelDay(d.Date.Day, d.IsOffDay, d.IsWithoutTotal)).ToArray(),
                    studentAttendances.Select(sa => new AttendancesModelStudent(studentsInfo[sa.Key].ClassNumber, studentsInfo[sa.Key].FullName, studentsInfo[sa.Key].IsTransferred, sa.Value, studentPresencesTotal[sa.Key])).ToArray(),
                    presencesDayTotals,
                    presencesCountTotal,
                    averagePresencesCount
                )
            );
        }

        return new(months.ToArray());
    }

    private async Task<PgResultsModel> GetPgResultsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var pgResults =
            await this.classBookPrintRepository.GetPgResultsAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            pgResults
            .Select(
                r => new PgResultsModelPgResult(
                    r.FullName,
                    r.CurriculumName,
                    r.StartSchoolYearResult,
                    r.EndSchoolYearResult))
            .ToArray());
    }

    private AbsencesCdoModel GetAbsencesCdoModel(int schoolYear, GetStudentsDataVO[] students, GetAbsencesByDateVO[] absencesByDate)
    {
        var studentsDict = students.ToDictionary(s => s.PersonId);

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

        Func<IEnumerable<GetAbsencesByDateVO>, AbsencesItemByTypes> aggregate =
            (absences) =>
            {
                var e = absences.Sum(gi => gi.ExcusedCount);
                var u = absences.Sum(gi => gi.UnexcusedCount);
                var l = absences.Sum(gi => gi.LateCount);
                return new AbsencesItemByTypes(e, u + l * 0.5M);
            };

        var groupedAbsences = absencesByDate
            .GroupBy(g1 => g1.PersonId)
            .Select(
                g1 =>
                {
                    var student = studentsDict[g1.Key];
                    return new AbsencesCdoModelAbsence(
                        student.ClassNumber,
                        StringUtils.JoinNames(student.FirstName, student.MiddleName, student.LastName),
                        student.IsTransferred,
                        aggregate(g1.Where(a => a.Date < oct1st)),
                        aggregate(g1.Where(a => oct1st <= a.Date && a.Date < nov1st)),
                        aggregate(g1.Where(a => nov1st <= a.Date && a.Date < dec1st)),
                        aggregate(g1.Where(a => dec1st <= a.Date && a.Date < jan1st)),
                        aggregate(g1.Where(a => jan1st <= a.Date && a.Date < feb1st)),
                        aggregate(g1.Where(a => feb1st <= a.Date && a.Date < mar1st)),
                        aggregate(g1.Where(a => mar1st <= a.Date && a.Date < apr1st)),
                        aggregate(g1.Where(a => apr1st <= a.Date && a.Date < may1st)),
                        aggregate(g1.Where(a => may1st <= a.Date && a.Date < jun1st)),
                        aggregate(g1.Where(a => jun1st <= a.Date && a.Date < jul1st)),
                        aggregate(g1.Where(a => jul1st <= a.Date && a.Date < aug1st)),
                        aggregate(g1.Where(a => a.Date >= aug1st))
                    );
                }
            )
            .OrderBy(a => a.ClassNumber ?? 99999);

        return new(groupedAbsences.ToArray());
    }

    private async Task<IndividualWorksModel> GetIndividualWorksModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var individualWorks =
            await this.classBookPrintRepository.GetIndividualWorksAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            individualWorks
            .Select(
                iw => new IndividualWorksModelIndividualWork(
                    iw.ClassNumber,
                    iw.FullName,
                    iw.IsTransferred,
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

    private async Task<ReplrParticipationsModel> GetReplrParticipationsModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var replrParticipations =
            await this.classBookPrintRepository.GetReplrParticipationsAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            replrParticipations
            .Select(
                rp => new ReplrParticipationsModelReplrParticipation(
                    rp.ReplrParticipationType,
                    rp.Date,
                    rp.Topic,
                    rp.InstName,
                    rp.Attendees))
            .ToArray());
    }

    private async Task<PerformancesModel> GetPerformancesModelAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var performances =
            await this.classBookPrintRepository.GetPerformancesAsync(
                schoolYear,
                classBookId,
                ct);

        return new(
            performances
            .Select(
                p => new PerformancesModelPerformance(
                    p.PerformanceType,
                    p.Name,
                    p.Description,
                    p.StartDate,
                    p.EndDate,
                    p.Location,
                    p.StudentAwards))
            .ToArray());
    }
}
