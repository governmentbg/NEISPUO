namespace SB.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ISchedulesQueryRepository;
using static SB.Domain.IStudentClassBooksQueryRepository;

internal class StudentClassBooksQueryRepository : Repository, IStudentClassBooksQueryRepository
{
    private const int NotApplicableSPPOOSpecialityId = -1;

    private ISchedulesQueryRepository schedulesQueryRepository;

    public StudentClassBooksQueryRepository(UnitOfWork unitOfWork, ISchedulesQueryRepository schedulesQueryRepository)
        : base(unitOfWork)
    {
        this.schedulesQueryRepository = schedulesQueryRepository;
    }

    public async Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(int schoolYear, int[] studentPersonIds, CancellationToken ct)
    {
        var query =
            from cb in this.DbContext.ClassBooksForStudents(schoolYear, studentPersonIds)
            join i in this.DbContext.Set<InstitutionSchoolYear>()
                on new { cb.InstId, cb.SchoolYear }
                equals new { InstId = i.InstitutionId, i.SchoolYear }
            join p in this.DbContext.Set<Person>()
                on cb.PersonId equals p.PersonId
            join bc in this.DbContext.Set<BasicClass>()
                on cb.BasicClassId equals bc.BasicClassId into j1
            from bc in j1.DefaultIfEmpty()
            select new
            {
                cb.SchoolYear,
                cb.InstId,
                InstName = i.Name,
                cb.ClassBookId,
                cb.IsValid,
                p.PersonId,
                cb.BookType,
                cb.BookName,
                cb.BasicClassId,
                BasicClassName = bc.Name,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                cb.PersonStatus,
                bc.SortOrd
            };

        var grouped = await query
            .GroupBy(x => new { x.SchoolYear, x.ClassBookId, x.PersonId })
            .Select(g => g
                .OrderBy(g => g.SortOrd)
                .ThenBy(g => g.BookName)
                .First()
            )
            .ToArrayAsync(ct);

        return grouped
            .Where(g => g.IsValid)
            .Select(g => new GetAllClassBooksVO(
                g.SchoolYear,
                g.InstId,
                g.InstName,
                g.ClassBookId,
                g.PersonId,
                g.BookType,
                g.BookName,
                g.BasicClassId,
                g.BasicClassName,
                g.FirstName,
                g.MiddleName,
                g.LastName,
                g.PersonStatus == StudentClassStatus.Transferred
            )).ToArray();
    }

    public async Task<GetClassBookInfoVO> GetClassBookInfoAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var cb = await (
            from c in this.DbContext.ClassBooksForStudents(schoolYear, new[] { personId })
            join bc in this.DbContext.Set<BasicClass>() on c.BasicClassId equals bc.BasicClassId into j1
            from bc in j1.DefaultIfEmpty()
            join cbsys in this.DbContext.Set<ClassBookSchoolYearSettings>()
                on new { c.SchoolYear, c.ClassBookId } equals new { cbsys.SchoolYear, cbsys.ClassBookId }
            join sp in this.DbContext.Set<SPPOOSpeciality>() on c.StudentSpecialityId equals sp.SPPOOSpecialityId into j2
            from sp in j2.DefaultIfEmpty()
            where c.SchoolYear == schoolYear && c.ClassBookId == classBookId
            select new
            {
                c.ClassBookId,
                c.BookType,
                c.BookName,
                c.BasicClassId,
                BasicClassName = bc.Name,
                c.IsIndividualCurriculum,
                cbsys.SchoolYearStartDateLimit,
                cbsys.SchoolYearStartDate,
                cbsys.FirstTermEndDate,
                cbsys.SecondTermStartDate,
                cbsys.SchoolYearEndDate,
                cbsys.SchoolYearEndDateLimit,
                SpecialityName = sp.SPPOOSpecialityId != NotApplicableSPPOOSpecialityId ? sp.Name : null
            }
       ).FirstAsync(ct);

        int daysCount;
        if (cb.BookType != ClassBookType.Book_PG)
        {
            daysCount = await (
                from a in this.DbContext.Set<Absence>()
                where a.SchoolYear == schoolYear &&
                      a.ClassBookId == classBookId &&
                      a.PersonId == personId &&
                      a.ExcusedReasonId == AbsenceReason.FamilyAbsenceReasonId
                select a.Date
            ).Distinct().CountAsync(ct);
        }
        else
        {
            daysCount = await (
                from a in this.DbContext.Set<Attendance>()
                where a.SchoolYear == schoolYear &&
                      a.ClassBookId == classBookId &&
                      a.PersonId == personId &&
                      a.ExcusedReasonId == AbsenceReason.FamilyAbsenceReasonId
                select a.Date
            ).Distinct().CountAsync(ct);
        }

        return new GetClassBookInfoVO(
            cb.ClassBookId,
            cb.BookType,
            cb.BookName,
            cb.BasicClassId,
            cb.BasicClassName,
            cb.IsIndividualCurriculum ?? false,
            cb.SchoolYearStartDateLimit,
            cb.SchoolYearStartDate,
            cb.FirstTermEndDate,
            cb.SecondTermStartDate,
            cb.SchoolYearEndDate,
            cb.SchoolYearEndDateLimit,
            cb.SpecialityName,
            daysCount
        );
    }

    public async Task<GetAbsencesVO[]> GetAbsencesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculum = this.ValidCurriculumStudent(schoolYear, classBook.ClassId, classBook.ClassIsLvl2, personId);

        var absences =
            from a in this.DbContext.Set<Absence>()
            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { a.SchoolYear, a.ScheduleLessonId }
            equals new { sl.SchoolYear, sl.ScheduleLessonId }
            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == personId
            group a by sl.CurriculumId into g
            select new
            {
                CurriculumId = g.Key,
                LateAbsencesCount = g.Sum(a => a.Type == AbsenceType.Late ? 1 : 0),
                UnexcusedAbsencesCount = g.Sum(a => a.Type == AbsenceType.Unexcused ? 1 : 0),
                ExcusedAbsencesCount = g.Sum(a => a.Type == AbsenceType.Excused ? 1 : 0),
            };

        // full outer join = (left outer join) union (right outer join)
        var absencesCurriculumFullOuterJoin =
            (from a in absences
             join curriculumId in curriculum on a.CurriculumId equals curriculumId into j1
             from curriculumId in j1.DefaultIfEmpty()
             select new
             {
                 CCCurriculumId = (int?)curriculumId,
                 ACurriculumId = (int?)a.CurriculumId,
                 LateAbsencesCount = (int?)a.LateAbsencesCount,
                 UnexcusedAbsencesCount = (int?)a.UnexcusedAbsencesCount,
                 ExcusedAbsencesCount = (int?)a.ExcusedAbsencesCount,
             }).Union(
                from curriculumId in curriculum
                join a in absences on curriculumId equals a.CurriculumId into j1
                from a in j1.DefaultIfEmpty()
                select new
                {
                    CCCurriculumId = (int?)curriculumId,
                    ACurriculumId = (int?)a.CurriculumId,
                    LateAbsencesCount = (int?)a.LateAbsencesCount,
                    UnexcusedAbsencesCount = (int?)a.UnexcusedAbsencesCount,
                    ExcusedAbsencesCount = (int?)a.ExcusedAbsencesCount,
                }
            ).Select(c =>
                new
                {
                    CurriculumId = c.CCCurriculumId ?? c.ACurriculumId ?? -1,
                    LateAbsencesCount = c.LateAbsencesCount ?? 0,
                    UnexcusedAbsencesCount = c.UnexcusedAbsencesCount ?? 0,
                    ExcusedAbsencesCount = c.ExcusedAbsencesCount ?? 0,
                });

        var absencesData = await (
            from ac in absencesCurriculumFullOuterJoin
            join ci in this.CurriculumInfoBase() on ac.CurriculumId equals ci.CurriculumId
            orderby ci.CurriculumPartId, ci.IsIndividualLesson, ci.TotalTermHours descending
            select new
            {
                ac.CurriculumId,
                ci.SubjectName,
                ci.SubjectTypeName,
                ci.CurriculumPartId,
                ci.IsIndividualLesson,
                ci.TotalTermHours,
                ac.LateAbsencesCount,
                ac.UnexcusedAbsencesCount,
                ac.ExcusedAbsencesCount
            }
        ).ToArrayAsync(ct);

        var curriculumIds = absencesData.Select(x => x.CurriculumId).Distinct();
        var teacherNames = await this.GetTeacherNamesByCurriculumAsync(curriculumIds, ct);

        return absencesData.Select(a => new GetAbsencesVO(
            a.CurriculumId,
            teacherNames.TryGetValue(a.CurriculumId, out var teachers) && !string.IsNullOrEmpty(teachers)
                ? $"{a.SubjectName} / {a.SubjectTypeName} - {teachers}"
                : $"{a.SubjectName} / {a.SubjectTypeName}",
            a.LateAbsencesCount,
            a.UnexcusedAbsencesCount,
            a.ExcusedAbsencesCount)
        ).ToArray();
    }

    public async Task<GetAbsencesDplrVO[]> GetAbsencesDplrAsync(
        int schoolYear,
        int classBookId,
        int personId,
        AbsenceType type,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculum = this.ValidCurriculumStudent(schoolYear, classBook.ClassId, classBook.ClassIsLvl2, personId);

        var absences =
            from a in this.DbContext.Set<Absence>()
            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { a.SchoolYear, a.ScheduleLessonId }
            equals new { sl.SchoolYear, sl.ScheduleLessonId }
            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == personId &&
                a.Type == type
            group a by sl.CurriculumId into g
            select new
            {
                CurriculumId = g.Key,
                Count = g.Count()
            };

        // full outer join = (left outer join) union (right outer join)
        var absencesCurriculumFullOuterJoin =
            (from a in absences
             join curriculumId in curriculum on a.CurriculumId equals curriculumId into j1
             from curriculumId in j1.DefaultIfEmpty()
             select new
             {
                 CCCurriculumId = (int?)curriculumId,
                 ACurriculumId = (int?)a.CurriculumId,
                 Count = (int?)a.Count,
             }).Union(
                from curriculumId in curriculum
                join a in absences on curriculumId equals a.CurriculumId into j1
                from a in j1.DefaultIfEmpty()
                select new
                {
                    CCCurriculumId = (int?)curriculumId,
                    ACurriculumId = (int?)a.CurriculumId,
                    Count = (int?)a.Count,
                }
            ).Select(c =>
                new
                {
                    CurriculumId = c.CCCurriculumId ?? c.ACurriculumId ?? -1,
                    Count = c.Count ?? 0
                });

        var absencesData = await (
            from ac in absencesCurriculumFullOuterJoin
            join ci in this.CurriculumInfoBase() on ac.CurriculumId equals ci.CurriculumId
            orderby ci.CurriculumPartId, ci.IsIndividualLesson, ci.TotalTermHours descending
            select new
            {
                ac.CurriculumId,
                ci.SubjectName,
                ci.SubjectTypeName,
                ac.Count
            }
        ).ToArrayAsync(ct);

        var curriculumIds = absencesData.Select(x => x.CurriculumId).Distinct();
        var teacherNames = await this.GetTeacherNamesByCurriculumAsync(curriculumIds, ct);

        return absencesData.Select(a => new GetAbsencesDplrVO(
            a.CurriculumId,
            teacherNames.TryGetValue(a.CurriculumId, out var teachers) && !string.IsNullOrEmpty(teachers)
                ? $"{a.SubjectName} / {a.SubjectTypeName} - {teachers}"
                : $"{a.SubjectName} / {a.SubjectTypeName}",
            a.Count)
        ).ToArray();
    }

    public async Task<GetAbsencesForCurriculumAndTypeVO[]> GetAbsencesForCurriculumAndTypeAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        AbsenceType type,
        CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Absence>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { a.SchoolYear, a.ScheduleLessonId }
            equals new { sl.SchoolYear, sl.ScheduleLessonId }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { a.SchoolYear, a.ScheduleLessonId, a.TeacherAbsenceId }
            equals new { tah.SchoolYear, tah.ScheduleLessonId, TeacherAbsenceId = (int?)tah.TeacherAbsenceId }
            into g1
            from tah in g1.DefaultIfEmpty()

            join ar in this.DbContext.Set<AbsenceReason>() on a.ExcusedReasonId equals ar.Id
            into g2
            from ar in g2.DefaultIfEmpty()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == personId &&
                sl.CurriculumId == curriculumId &&
                a.Type == type

            orderby a.Date, a.CreateDate

            select new GetAbsencesForCurriculumAndTypeVO(
                a.AbsenceId,
                s.SubjectName,
                s.SubjectNameShort,
                st.Name,
                a.Date,
                a.Type,
                EnumUtils.GetEnumDescription(a.Type),
                a.ExcusedReasonId,
                ar.Name,
                a.ExcusedReasonComment,
                tah.ReplTeacherIsNonSpecialist,
                a.IsReadFromParent)
        ).ToArrayAsync(ct);
    }

    public async Task<GetAttendanceMonthStatsVO[]> GetAttendancesMonthStatsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var schoolYearLimitDates = await (
            from sy in this.DbContext.Set<ClassBookSchoolYearSettings>()

            where sy.SchoolYear == schoolYear &&
                sy.ClassBookId == classBookId

            select new
            {
                sy.SchoolYearStartDate,
                sy.SchoolYearEndDate
            }
        ).SingleAsync(ct);

        var attendancesGrouped = await (
            from a in this.DbContext.Set<Attendance>()

            where
                a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == personId

            group a by new { Year = a.Date.Year, Month = a.Date.Month, Type = a.Type } into g

            select new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Type = g.Key.Type,
                Count = g.Count(),
            }
        ).ToDictionaryAsync(g => (g.Year, g.Month, g.Type), g => g.Count, ct);

        return DateExtensions
            .GetMonthsInRange(
                schoolYearLimitDates.SchoolYearStartDate,
                DateExtensions.Min(DateTime.Now, schoolYearLimitDates.SchoolYearEndDate))
            .Select(((int year, int month) t) =>
                new GetAttendanceMonthStatsVO(
                    t.year,
                    t.month,
                    attendancesGrouped.GetValueOrDefault((t.year, t.month, AttendanceType.Presence)),
                    attendancesGrouped.GetValueOrDefault((t.year, t.month, AttendanceType.UnexcusedAbsence)),
                    attendancesGrouped.GetValueOrDefault((t.year, t.month, AttendanceType.ExcusedAbsence))
                ))
            .ToArray();
    }

    public async Task<GetAttendancesVO[]> GetAttendancesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        AttendanceType type,
        int year,
        int month,
        CancellationToken ct)
    {
        DateTime monthStartDate = new DateTime(year, month, 1);
        DateTime monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

        return await (
            from a in this.DbContext.Set<Attendance>()
            join ar in this.DbContext.Set<AbsenceReason>() on a.ExcusedReasonId equals ar.Id
            into g2
            from ar in g2.DefaultIfEmpty()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId &&
                a.PersonId == personId &&
                a.Type == type &&
                a.Date >= monthStartDate &&
                a.Date <= monthEndDate

            select new GetAttendancesVO(
                a.Date,
                a.Type,
                EnumUtils.GetEnumDescription(a.Type),
                ar.Name,
                a.ExcusedReasonComment)
        ).ToArrayAsync(ct);
    }

    public async Task<TableResultVO<GetExamsVO>> GetExamsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from e in this.DbContext.Set<Exam>()

            join c in this.DbContext.Set<Curriculum>() on e.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId

            orderby e.Date descending

            select new GetExamsVO(
                e.Type,
                $"{s.SubjectName} / {st.Name}",
                e.Date,
                e.Description))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetIndividualWorksVO>> GetIndividualWorksAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from iw in this.DbContext.Set<IndividualWork>()

            join csu in this.DbContext.Set<SysUser>() on iw.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            where iw.SchoolYear == schoolYear &&
                iw.ClassBookId == classBookId &&
                iw.PersonId == personId

            orderby iw.Date

            select new GetIndividualWorksVO(
                iw.Date,
                iw.IndividualWorkActivity,
                StringUtils.JoinNames(cp.FirstName, cp.MiddleName, cp.LastName)))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetGradeResultVO> GetGradeResultAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var sessions = await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
            on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }
            into g1
            from grs in g1.DefaultIfEmpty()

            join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId
            into g2
            from c in g2.DefaultIfEmpty()

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            into g3
            from s in g3.DefaultIfEmpty()

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId
            into g4
            from st in g4.DefaultIfEmpty()

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId &&
                gr.PersonId == personId

            select new
            {
                gr.InitialResultType,
                gr.FinalResultType,
                Curriculum = string.Join(" / ", s.SubjectName, st.Name),
                grs.Session1NoShow,
                Session1Grade = GradeUtils.GetDecimalGradeText(grs.Session1Grade),
                grs.Session2NoShow,
                Session2Grade = GradeUtils.GetDecimalGradeText(grs.Session2Grade),
                grs.Session3NoShow,
                Session3Grade = GradeUtils.GetDecimalGradeText(grs.Session3Grade)
            })
            .ToArrayAsync(ct);

        return new GetGradeResultVO(
            sessions.Length > 0 ? sessions[0].InitialResultType : null,
            sessions.Length > 0 ? sessions[0].FinalResultType : null,
            sessions.Select(
                s => new GetGradeResultVOSession(
                    s.Curriculum,
                    s.Session1NoShow,
                    s.Session1Grade,
                    s.Session2NoShow,
                    s.Session2Grade,
                    s.Session3NoShow,
                    s.Session3Grade
                )
            ).ToArray()
        );
    }

    public async Task<GetFirstGradeResultsVO?> GetFirstGradeResultsOrDefaultAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<FirstGradeResult>()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.PersonId == personId

            select new GetFirstGradeResultsVO(
                r.QualitativeGrade,
                r.SpecialGrade))
            .SingleOrDefaultAsync(ct);
    }

    [Keyless]
    private record GradeVO(
        [property: Column(TypeName = "SMALLINT")] int SchoolYear,
        int ClassBookId,
        int PersonId,
        int CurriculumId,
        int GradeId,
        GradeCategory Category,
        GradeType Type,
        SchoolTerm Term,
        [property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        DateTime GradeDate,
        DateTime CreateDate,
        bool IsReadFromParent);

    public async Task<GetGradesVO[]> GetGradesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculum =
            from curriculumId in this.ValidCurriculumStudent(schoolYear, classBook.ClassId, classBook.ClassIsLvl2, personId)

            join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(cg => cg.SchoolYear == schoolYear && cg.ClassBookId == classBookId)
            on curriculumId equals cg.CurriculumId
            into j1
            from cg in j1.DefaultIfEmpty()

            where cg == null

            select curriculumId;

        string sql = """
            SELECT
                g.SchoolYear,
                g.ClassBookId,
                g.PersonId,
                g.CurriculumId,
                g.GradeId,
                g.Date as GradeDate,
                g.Type,
                g.Term,
                g.Category,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade,
                g.CreateDate,
                g.IsReadFromParent
            FROM
                [school_books].[Grade] g
            """;
        var studentGrades =
            from g in this.DbContext.Set<GradeVO>().FromSqlRaw(sql)
            where g.SchoolYear == schoolYear &&
                g.ClassBookId == classBookId &&
                g.PersonId == personId
            select g;

        var studentGradesCurriculumUnion =
            (from g in studentGrades
             select g.CurriculumId
            ).Union(curriculum);

        var orderedCurriculumWithInfo = await (
            from curriculumId in studentGradesCurriculumUnion
            join ci in this.CurriculumInfoBase() on curriculumId equals ci.CurriculumId
            orderby ci.CurriculumPartId, ci.IsIndividualLesson, ci.TotalTermHours descending
            select new
            {
                ci.CurriculumId,
                ci.SubjectName,
                ci.SubjectTypeName
            }
        ).ToArrayAsync(ct);

        var curriculumIds = orderedCurriculumWithInfo.Select(x => x.CurriculumId).Distinct();
        var teacherNames = await this.GetTeacherNamesByCurriculumAsync(curriculumIds, ct);

        var studentGradesByCourse =
            (await studentGrades.ToListAsync(ct))
            .ToLookup(g => g.CurriculumId);

        return (
            from cc in orderedCurriculumWithInfo
            let curriculumName = teacherNames.TryGetValue(cc.CurriculumId, out var teachers) && !string.IsNullOrEmpty(teachers)
                ? $"{cc.SubjectName} / {cc.SubjectTypeName} - {teachers}"
                : $"{cc.SubjectName} / {cc.SubjectTypeName}"
            let grades =
                studentGradesByCourse[cc.CurriculumId]
                .OrderBy(g => g.GradeDate)
                .ThenBy(g => g.CreateDate)
                .Select(g => new GetGradesVOGrade(
                    g.CurriculumId,
                    g.GradeId,
                    g.Category,
                    g.Type,
                    g.Term,
                    g.DecimalGrade,
                    g.QualitativeGrade,
                    g.SpecialGrade,
                    g.GradeDate,
                    g.IsReadFromParent))
            select new GetGradesVO(
                cc.CurriculumId,
                curriculumName,
                    grades.Where(g =>
                        g.Term == SchoolTerm.TermOne &&
                        g.Type != GradeType.Term &&
                        g.Type != GradeType.Final &&
                        g.Type != GradeType.OtherClassTerm &&
                        g.Type != GradeType.OtherSchoolTerm)
                    .OrderBy(g => g.GradeDate)
                    .ThenBy(g => g.GradeId)
                    .ToArray(),
                    grades.Where(g =>
                        g.Term == SchoolTerm.TermOne &&
                        (g.Type == GradeType.Term || g.Type == GradeType.OtherClassTerm || g.Type == GradeType.OtherSchoolTerm))
                    .OrderBy(g => g.GradeDate)
                    .ThenBy(g => g.GradeId)
                    .ToArray(),
                    grades.Where(g =>
                        g.Term == SchoolTerm.TermTwo &&
                        g.Type != GradeType.Term &&
                        g.Type != GradeType.Final &&
                        g.Type != GradeType.OtherClassTerm &&
                        g.Type != GradeType.OtherSchoolTerm)
                    .OrderBy(g => g.GradeDate)
                    .ThenBy(g => g.GradeId)
                    .ToArray(),
                    grades.Where(g =>
                        g.Term == SchoolTerm.TermTwo &&
                        (g.Type == GradeType.Term || g.Type == GradeType.OtherClassTerm || g.Type == GradeType.OtherSchoolTerm))
                    .OrderBy(g => g.GradeDate)
                    .ThenBy(g => g.GradeId)
                    .ToArray(),
                    grades.Where(g => g.Type == GradeType.Final)
                    .OrderBy(g => g.GradeDate)
                    .ThenBy(g => g.GradeId)
                    .ToArray()
            )
        ).ToArray();
    }

    public async Task<GetGradeVO> GetGradeAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int gradeId,
        CancellationToken ct)
    {
        string sql = """
            SELECT
                g.GradeId,
                s.SubjectId,
                s.SubjectName,
                s.SubjectNameShort,
                st.SubjectTypeId,
                st.Name AS SubjectTypeName,
                bst.Abrev AS BasicSubjectTypeNameShort,
                CAST(IIF(bst.BasicSubjectTypeId IN (1, 11, 5), 1, 0) AS BIT) AS BasicSubjectTypeIsMandatoryCurriculum,
                g.Date,
                g.Type,
                g.Term,
                g.Category,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade,
                g.Comment
            FROM
                [school_books].[Grade] g
                INNER JOIN [inst_year].[Curriculum] c ON g.[CurriculumId] = c.[CurriculumId]
                INNER JOIN [inst_nom].[Subject] s ON c.[SubjectId] = s.[SubjectId]
                INNER JOIN [inst_nom].[SubjectType] st ON c.[SubjectTypeId] = st.[SubjectTypeId]
                INNER JOIN [inst_nom].[BasicSubjectType] bst ON st.[BasicSubjectTypeId] = bst.[BasicSubjectTypeId]
            WHERE
                g.SchoolYear = @schoolYear AND
                g.ClassBookId = @classBookId AND
                g.PersonId = @personId AND
                g.GradeId = @gradeId
            """;

        return await this.DbContext.Set<GetGradeVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("personId", personId),
                new SqlParameter("gradeId", gradeId))
            .SingleAsync(ct);
    }

    public async Task<GetTopicsVO[]> GetTopicsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        CancellationToken ct)
    {
        var bookType = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select
                cb.BookType
        ).SingleAsync(ct);

        if (bookType != ClassBookType.Book_I_III &&
            bookType != ClassBookType.Book_IV &&
            bookType != ClassBookType.Book_V_XII)
        {
            throw new Exception("Student topics are allowed only for book types I-III, IV, V-XII.");
        }

        return (await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join t in this.DbContext.Set<Topic>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { t.SchoolYear, t.ScheduleLessonId }
            into g1
            from t in g1.DefaultIfEmpty()

            join od in this.DbContext.Set<ClassBookOffDayDate>()
            on new { sch.SchoolYear, sch.ClassBookId, sl.Date }
            equals new { od.SchoolYear, od.ClassBookId, od.Date }
            into g2
            from od in g2.DefaultIfEmpty()

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId &&
                sl.CurriculumId == curriculumId

            orderby sl.Date, sl.HourNumber

            select new
            {
                sl.Date,
                IsOffDay = od != null,
                Titles = t.Titles.Select(tt => tt.Title).ToArray()
            }
        ).ToListAsync(ct))
        .Select((t, index) => new GetTopicsVO(index + 1, t.Date, t.IsOffDay, t.Titles))
        .ToArray();
    }

    public async Task<TableResultVO<GetNotesVO>> GetNotesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from n in this.DbContext.Set<Note>()
            join csu in this.DbContext.Set<SysUser>() on n.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId &&
                (n.Students.Any(s => s.PersonId == personId) || n.IsForAllStudents)

            orderby n.NoteId

            select new GetNotesVO(
                n.CreateDate,
                StringUtils.JoinNames(cp.FirstName, cp.MiddleName, cp.LastName),
                n.Description))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetParentMeetingsVO>> GetParentMeetingsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ParentMeeting>()
            .Where(pm =>
                pm.SchoolYear == schoolYear &&
                pm.ClassBookId == classBookId)
            .OrderByDescending(pm => pm.Date)
            .Select(pm => new GetParentMeetingsVO(
                pm.Date,
                pm.StartTime,
                pm.Location,
                pm.Title,
                pm.Description))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetPgResultsVO>> GetPgResultsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from pgr in this.DbContext.Set<PgResult>()

            join s in this.DbContext.Set<Subject>() on pgr.SubjectId equals s.SubjectId
            into g2
            from s in g2.DefaultIfEmpty()

            where pgr.SchoolYear == schoolYear &&
                pgr.ClassBookId == classBookId &&
                pgr.PersonId == personId

            orderby pgr.CreateDate

            select new GetPgResultsVO(
                pgr.PgResultId,
                s.SubjectName,
                pgr.StartSchoolYearResult,
                pgr.EndSchoolYearResult))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetRemarksVO[]> GetRemarksAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculum = this.ValidCurriculumStudent(schoolYear, classBook.ClassId, classBook.ClassIsLvl2, personId);

        var remarks =
            from r in this.DbContext.Set<Remark>()
            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.PersonId == personId
            group r by r.CurriculumId into g
            select new
            {
                CurriculumId = g.Key,
                BadRemarksCount = g.Count(x => x.Type == RemarkType.Bad),
                GoodRemarksCount = g.Count(y => y.Type == RemarkType.Good)
            };

        // full outer join = (left outer join) union (right outer join)
        var absencesCurriculumFullOuterJoin =
            (from r in remarks
             join curriculumId in curriculum on r.CurriculumId equals curriculumId into j1
             from curriculumId in j1.DefaultIfEmpty()
             select new
             {
                 CCCurriculumId = (int?)curriculumId,
                 RCurriculumId = (int?)r.CurriculumId,
                 BadRemarksCount = (int?)r.BadRemarksCount,
                 GoodRemarksCount = (int?)r.GoodRemarksCount,
             }).Union(
                from curriculumId in curriculum
                join r in remarks on curriculumId equals r.CurriculumId into j1
                from r in j1.DefaultIfEmpty()
                select new
                {
                    CCCurriculumId = (int?)curriculumId,
                    RCurriculumId = (int?)r.CurriculumId,
                    BadRemarksCount = (int?)r.BadRemarksCount,
                    GoodRemarksCount = (int?)r.GoodRemarksCount,
                }
            ).Select(c =>
                new
                {
                    CurriculumId = c.CCCurriculumId ?? c.RCurriculumId ?? -1,
                    BadRemarksCount = c.BadRemarksCount ?? 0,
                    GoodRemarksCount = c.GoodRemarksCount ?? 0,
                });

        var remarksData = await (
            from ac in absencesCurriculumFullOuterJoin
            join ci in this.CurriculumInfoBase() on ac.CurriculumId equals ci.CurriculumId
            orderby ci.CurriculumPartId, ci.IsIndividualLesson, ci.TotalTermHours descending
            select new
            {
                ac.CurriculumId,
                ci.SubjectName,
                ci.SubjectTypeName,
                ac.BadRemarksCount,
                ac.GoodRemarksCount
            }
        ).ToArrayAsync(ct);

        var curriculumIds = remarksData.Select(x => x.CurriculumId).Distinct();
        var teacherNames = await this.GetTeacherNamesByCurriculumAsync(curriculumIds, ct);

        return remarksData.Select(r => new GetRemarksVO(
            r.CurriculumId,
            teacherNames.TryGetValue(r.CurriculumId, out var teachers) && !string.IsNullOrEmpty(teachers)
                ? $"{r.SubjectName} / {r.SubjectTypeName} - {teachers}"
                : $"{r.SubjectName} / {r.SubjectTypeName}",
            r.BadRemarksCount,
            r.GoodRemarksCount)
        ).ToArray();
    }

    public async Task<GetRemarksForCurriculumAndTypeVO[]> GetRemarksForCurriculumAndTypeAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        RemarkType type,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<Remark>()

            join c in this.DbContext.Set<Curriculum>() on r.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.PersonId == personId &&
                r.CurriculumId == curriculumId &&
                r.Type == type

            orderby r.Date, r.CreateDate

            select new GetRemarksForCurriculumAndTypeVO(
                r.RemarkId,
                s.SubjectName,
                s.SubjectNameShort,
                st.Name,
                r.Date,
                r.Type,
                EnumUtils.GetEnumDescription(r.Type),
                r.Description,
                r.IsReadFromParent)
        ).ToArrayAsync(ct);
    }

    public async Task<TableResultVO<GetSanctionsVO>> GetSanctionsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Sanction>()
            join st in this.DbContext.Set<SanctionType>() on s.SanctionTypeId equals st.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.PersonId == personId

            orderby s.SanctionId

            select new GetSanctionsVO(
                st.Name,
                s.StartDate,
                s.EndDate,
                s.Description))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetClassBookScheduleTableForWeekVO> GetScheduleAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int year,
        int weekNumber,
        bool showIndividualCurriculum,
        CancellationToken ct)
    {
        return await this.schedulesQueryRepository.GetClassBookScheduleTableForWeekAsync(
            schoolYear,
            classBookId,
            year,
            weekNumber,
            showIndividualCurriculum,
            personId,
            showOnlyStudentCurriculums: true,
            ct);
    }

    public async Task<TableResultVO<GetSupportsVO>> GetSupportsAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var activities = (await (
            from s in this.DbContext.Set<Support>()

            join a in this.DbContext.Set<SupportActivity>()
                on new { s.SchoolYear, s.SupportId }
                equals new { a.SchoolYear, a.SupportId }

            join sat in this.DbContext.Set<SupportActivityType>() on a.SupportActivityTypeId equals sat.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby a.SupportActivityId

            select new
            {
                s.SupportId,
                sat.Name
            })
            .ToArrayAsync(ct))
            .ToLookup(
                a => a.SupportId,
                a => a.Name);

        var difficulties = (await (
            from s in this.DbContext.Set<Support>()

            join d in this.DbContext.Set<SupportDifficulty>()
                on new { s.SchoolYear, s.SupportId }
                equals new { d.SchoolYear, d.SupportId }

            join sdt in this.DbContext.Set<SupportDifficultyType>() on d.SupportDifficultyTypeId equals sdt.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new
            {
                s.SupportId,
                sdt.Name
            })
            .ToArrayAsync(ct))
            .ToLookup(
                d => d.SupportId,
                d => d.Name);

        return await (
            from s in this.DbContext.Set<Support>()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                (s.Students.Any(s => s.PersonId == personId) || !s.Students.Any())

            orderby s.SupportId

            select new GetSupportsVO(
                s.SupportId,
                string.Join(", ", difficulties[s.SupportId]),
                string.Join(", ", activities[s.SupportId]),
                s.EndDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetSupportVO> GetSupportAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int supportId,
        CancellationToken ct)
    {
        var teacherNames = await (
            from st in this.DbContext.Set<SupportTeacher>()
            join p in this.DbContext.Set<Person>() on st.PersonId equals p.PersonId
            where st.SchoolYear == schoolYear && st.SupportId == supportId
            select StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)
        ).ToArrayAsync(ct);

        var difficultyTypeNames = await (
            from sd in this.DbContext.Set<SupportDifficulty>()
            join sdt in this.DbContext.Set<SupportDifficultyType>() on sd.SupportDifficultyTypeId equals sdt.Id
            where sd.SchoolYear == schoolYear && sd.SupportId == supportId
            select sdt.Name
        ).ToArrayAsync(ct);

        return await (
            from s in this.DbContext.Set<Support>()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.SupportId == supportId &&
                (s.Students.Any(s => s.PersonId == personId) || !s.Students.Any())

            select new GetSupportVO(
                s.SupportId,
                string.Join(", ", difficultyTypeNames),
                s.Description,
                s.ExpectedResult,
                s.EndDate,
                string.Join(", ", teacherNames))
        ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetSupportActivitiesVO>> GetSupportActivitiesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int supportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from sa in this.DbContext.Set<SupportActivity>()
            join sat in this.DbContext.Set<SupportActivityType>() on sa.SupportActivityTypeId equals sat.Id

            join s in this.DbContext.Set<Support>()
                on new { sa.SchoolYear, sa.SupportId }
                equals new { s.SchoolYear, s.SupportId }

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.SupportId == supportId &&
                (s.Students.Any(s => s.PersonId == personId) || !s.Students.Any())

            orderby sa.SupportActivityId

            select new GetSupportActivitiesVO(
                sat.Name,
                sa.Date,
                sa.Target,
                sa.Result)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetCurriculumForTopicsVO[]> GetCurriculumForTopicsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculum =
            this.ValidCurriculumClass(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            .Union(this.ValidCurriculumStudent(schoolYear, classBook.ClassId, classBook.ClassIsLvl2, personId));

        return await (
            from curriculumId in curriculum

            join c in this.DbContext.Set<Curriculum>() on curriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            select new GetCurriculumForTopicsVO(
                c.CurriculumId,
                s.SubjectName,
                s.SubjectNameShort,
                (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                st.Name)
        ).ToArrayAsync(ct);
    }

    private IQueryable<int> ValidCurriculumClass(int schoolYear, int classBookClassId, bool classBookClassIsLvl2)
        => from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBookClassId, classBookClassIsLvl2)
           where cc.IsValid
           select cc.CurriculumId;

    private IQueryable<int> ValidCurriculumStudent(int schoolYear, int classBookClassId, bool classBookClassIsLvl2, int personId)
        => from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBookClassId, classBookClassIsLvl2)
           join cs in this.DbContext.Set<CurriculumStudent>() on cc.CurriculumId equals cs.CurriculumId
           where cc.IsValid &&
               cs.PersonId == personId &&
               cs.IsValid
           select cc.CurriculumId;

    private async Task<Dictionary<int, string>> GetTeacherNamesByCurriculumAsync(IEnumerable<int> curriculumIds, CancellationToken ct)
    {
        var curriculumIdsList = curriculumIds.ToList();

        if (!curriculumIdsList.Any())
        {
            return new Dictionary<int, string>();
        }

        var teacherData = await (
            from ct1 in this.DbContext.Set<CurriculumTeacher>()

            join sp in this.DbContext.Set<StaffPosition>()
                on ct1.StaffPositionId equals sp.StaffPositionId
            join p in this.DbContext.Set<Person>()
                on sp.PersonId equals p.PersonId

            where curriculumIdsList.Contains(ct1.CurriculumId) &&
                  ct1.IsValid &&
                  sp.IsValid

            select new
            {
                ct1.CurriculumId,
                TeacherName = p.FirstName + " " + p.LastName
            }
        ).ToListAsync(ct);

        return teacherData
            .GroupBy(t => t.CurriculumId)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ", g.Take(2).Select(t => t.TeacherName))
            );
    }

    private IQueryable<CurriculumInfoBaseVO> CurriculumInfoBase()
        => from c in this.DbContext.Set<Curriculum>()
           join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
           join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId
           select new CurriculumInfoBaseVO
           {
               CurriculumId = c.CurriculumId,
               SubjectName = s.SubjectName,
               SubjectTypeName = st.Name,
               CurriculumPartId = c.CurriculumPartID,
               IsIndividualLesson = c.IsIndividualLesson,
               TotalTermHours = c.TotalTermHours
           };

    [Keyless]
    private record CurriculumTeacherNamesVO(int CurriculumId, string TeacherNames);

    private record CurriculumInfoBaseVO()
    {
        public int CurriculumId { get; init; }
        public required string SubjectName { get; init; }
        public required string SubjectTypeName { get; init; }
        public int? CurriculumPartId { get; init; }
        public int? IsIndividualLesson { get; init; }
        public int? TotalTermHours { get; init; }
    }

    private record CurriculumInfoVO()
    {
        public int CurriculumId { get; init; }
        public required string CurriculumName { get; init; }
        public int? CurriculumPartId { get; init; }
        public int? IsIndividualLesson { get; init; }
        public int? TotalTermHours { get; init; }
    }
}
