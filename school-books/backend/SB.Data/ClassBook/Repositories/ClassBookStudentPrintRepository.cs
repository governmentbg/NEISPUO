namespace SB.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IClassBookStudentPrintRepository;

internal class ClassBookStudentPrintRepository : Repository, IClassBookStudentPrintRepository
{
    public ClassBookStudentPrintRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetStudentCoverPageDataVO> GetCoverPageDataAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var studentName = await (
            from p in this.DbContext.Set<Person>()
            where p.PersonId == personId
            select StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)
        ).SingleAsync(ct);

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId

            join s in this.DbContext.Set<SPPOOSpeciality>() on cg.ClassSpecialityId equals s.SPPOOSpecialityId
            into j0
            from s in j0.DefaultIfEmpty()

            join sy in this.DbContext.Set<InstitutionSchoolYear>()
            on new { cb.InstId, cb.SchoolYear } equals new { InstId = sy.InstitutionId, sy.SchoolYear }

            join la in this.DbContext.Set<LocalArea>() on sy.LocalAreaId equals la.LocalAreaId
            into j1
            from la in j1.DefaultIfEmpty()

            join t in this.DbContext.Set<Town>() on sy.TownId equals t.TownId
            into j2
            from t in j2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into j3
            from m in j3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into j4
            from r in j4.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear
                && cb.ClassBookId == classBookId

            select new GetStudentCoverPageDataVO(
                studentName,
                cg.BasicClassId,
                cb.BookType,
                sy.Abbreviation,
                t.Type == "1" ? "гр." :
                    t.Type == "2" ? "с." :
                    null,
                t.Name,
                m.Name,
                la.Name,
                r.Name,
                cg.ClassName,
                null,
                s.Name)
        ).SingleAsync(ct);
    }

    public async Task<GetStudentTeacherSubjectsVO[]> GetTeacherSubjectsAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var curriculumTeachers = (await (
                from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)

                join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

                join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

                join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId

                orderby p.FirstName, p.LastName

                where cc.IsValid

                select new
                {
                    cc.CurriculumId,
                    Name = StringUtils.JoinNames(p.FirstName, p.LastName)
                })
                .ToArrayAsync(ct))
                .ToLookup(t => t.CurriculumId, t => t.Name);

        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            where
                cc.IsValid &&
                c.IsValid

            select new GetStudentTeacherSubjectsVO(
                s.SubjectName,
                st.Name,
                string.Join(", ", curriculumTeachers[c.CurriculumId]))
        ).ToArrayAsync(ct);
    }

    public async Task<GetStudentSchedulesDataVO[]> GetSchedulesDataAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Schedule>()

            where s.SchoolYear == schoolYear
                && s.ClassBookId == classBookId

            orderby s.EndDate

            select new GetStudentSchedulesDataVO(
                s.ScheduleId,
                s.StartDate,
                s.EndDate)
        ).ToArrayAsync(ct);
    }

    public async Task<GetStudentSchedulesLessonsVO[]> GetSchedulesLessonsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where sch.SchoolYear == schoolYear
                && sch.ClassBookId == classBookId

            select new GetStudentSchedulesLessonsVO(
                sch.ScheduleId,
                sl.Day,
                sl.HourNumber,
                s.SubjectName,
                st.Name)
        ).ToArrayAsync(ct);
    }

    public async Task<GetStudentParentMeetingsVO[]> GetParentMeetingsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from pm in this.DbContext.Set<ParentMeeting>()

            where pm.SchoolYear == schoolYear
                && pm.ClassBookId == classBookId

            orderby pm.Date, pm.StartTime

            select new GetStudentParentMeetingsVO(
                pm.Date,
                pm.Title,
                pm.Description)
        ).ToArrayAsync(ct);
    }

    public async Task<GetStudentExamsVO[]> GetExamsAsync(int schoolYear, int classBookId, BookExamType type, CancellationToken ct)
    {
        var schoolYearSettings = await (
            from sy in this.DbContext.Set<ClassBookSchoolYearSettings>()
            where sy.SchoolYear == schoolYear && sy.ClassBookId == classBookId
            select new
            {
                sy.SchoolYearStartDate,
                sy.FirstTermEndDate,
                sy.SecondTermStartDate,
                sy.SchoolYearEndDate
            }
        ).SingleAsync(ct);

        var exams = await (
            from e in this.DbContext.Set<Exam>()

            join c in this.DbContext.Set<Curriculum>() on e.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId &&
                e.Type == type

            orderby e.Date descending

            select new
            {
                e.Type,
                s.SubjectName,
                SubjectTypeName = st.Name,
                e.Date
            }).ToArrayAsync(ct);

        return exams
            .GroupBy(e => new { e.SubjectName, e.SubjectTypeName })
            .OrderBy(g => g.Key.SubjectName)
            .ThenBy(g => g.Key.SubjectTypeName)
            .Select(g => new GetStudentExamsVO(
                g.Key.SubjectName,
                g.Key.SubjectTypeName,
                g.Where(gi => gi.Date < schoolYearSettings.SecondTermStartDate)
                .OrderBy(gi => gi.Date)
                .Select(gi => gi.Date)
                .ToArray(),
                g.Where(gi => gi.Date >= schoolYearSettings.SecondTermStartDate)
                .OrderBy(gi => gi.Date)
                .Select(gi => gi.Date)
                .ToArray()
                ))
            .ToArray();
    }

    public async Task<GetStudentGradesVO[]> GetGradesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var gradesSql =
$@"
SELECT
    c.CurriculumId,
    pc.CurriculumId AS ParentCurriculumId,
    s.SubjectId,

    CASE 
        WHEN pc.CurriculumId IS NOT NULL 
            THEN ps.SubjectName + '(ПП) -> ' + s.SubjectName
        ELSE s.SubjectName
    END
    + ' / ' + st.Name
    + CASE WHEN c.IsIndividualCurriculum = 1 THEN ' (ИУП)' ELSE '' END
    + CASE WHEN c.IsIndividualLesson = 1 THEN ' (ИЧ)' ELSE '' END
    AS CurriculumName,

    st.SubjectTypeId,
    c.CurriculumPartID,
    c.IsIndividualLesson,
    c.TotalTermHours,
    c.IsValid AS CurriculumIsValid,
    g.Date AS GradeDate,
    g.Type,
    g.Term,
    g.Category,
    g.DecimalGrade,
    g.QualitativeGrade,
    g.SpecialGrade
FROM
    [school_books].[Grade] g
    INNER JOIN [inst_year].[Curriculum] c ON g.[CurriculumId] = c.[CurriculumId]
    INNER JOIN [inst_nom].[Subject] s ON c.[SubjectId] = s.[SubjectId]
    INNER JOIN [inst_nom].[SubjectType] st ON c.[SubjectTypeId] = st.[SubjectTypeId]

    LEFT JOIN [inst_year].[Curriculum] pc 
        ON c.ParentCurriculumID = pc.CurriculumId 
        AND c.CurriculumId != c.ParentCurriculumID

    LEFT JOIN [inst_nom].[Subject] ps 
        ON pc.SubjectId = ps.SubjectId

WHERE
    g.SchoolYear = @schoolYear AND
    g.ClassBookId = @classBookId AND
    g.PersonId = @personId
ORDER BY
    g.Date,
    g.CreateDate
";
        return await this.DbContext.Set<GetStudentGradesVO>()
            .FromSqlRaw(gradesSql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("personId", personId))
            .ToArrayAsync(ct);
    }

    public async Task<GetStudentRemarksVO[]> GetRemarksAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var schoolYearSettings = await (
            from sy in this.DbContext.Set<ClassBookSchoolYearSettings>()
            where sy.SchoolYear == schoolYear && sy.ClassBookId == classBookId
            select new
            {
                sy.SchoolYearStartDate,
                sy.FirstTermEndDate,
                sy.SecondTermStartDate,
                sy.SchoolYearEndDate
            }
        ).SingleAsync(ct);

        return await (
            from r in this.DbContext.Set<Remark>()

            join c in this.DbContext.Set<Curriculum>() on r.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.PersonId == personId

            orderby r.Date descending

            select new GetStudentRemarksVO(
                r.Date < schoolYearSettings.SecondTermStartDate ? SchoolTerm.TermOne : SchoolTerm.TermTwo,
                r.Date,
                s.SubjectName,
                st.Name,
                $"{r.Type.GetEnumDescription()} {(string.IsNullOrEmpty(r.Description) ? string.Empty : " - " + r.Description)}")
            ).ToArrayAsync(ct);
    }

    public async Task<GetStudentAbsencesVO?> GetAbsencesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var sql =
$@"
WITH Absences AS (
    SELECT
        PersonId,
        SUM(IIF(Type = {(int)AbsenceType.Late} AND Term = {(int)SchoolTerm.TermOne}, 1, 0)) AS FirstTermLateCount,
        SUM(IIF(Type = {(int)AbsenceType.Unexcused} AND Term = {(int)SchoolTerm.TermOne}, 1, 0)) AS FirstTermUnexcusedCount,
        SUM(IIF(Type = {(int)AbsenceType.Excused} AND Term = {(int)SchoolTerm.TermOne}, 1, 0)) AS FirstTermExcusedCount,
        SUM(IIF(Type = {(int)AbsenceType.Late} AND Term = {(int)SchoolTerm.TermTwo}, 1, 0)) AS SecondTermLateCount,
        SUM(IIF(Type = {(int)AbsenceType.Unexcused} AND Term = {(int)SchoolTerm.TermTwo}, 1, 0)) AS SecondTermUnexcusedCount,
        SUM(IIF(Type = {(int)AbsenceType.Excused} AND Term = {(int)SchoolTerm.TermTwo}, 1, 0)) AS SecondTermExcusedCount,
        SUM(IIF(Type = {(int)AbsenceType.Late}, 1, 0)) AS WholeYearLateCount,
        SUM(IIF(Type = {(int)AbsenceType.Unexcused}, 1, 0)) AS WholeYearUnexcusedCount,
        SUM(IIF(Type = {(int)AbsenceType.Excused}, 1, 0)) AS WholeYearExcusedCount
    FROM
        [school_books].[Absence]
    WHERE
        SchoolYear = @schoolYear AND ClassBookId = @classBookId AND PersonId = @personId
    GROUP BY
        PersonId
),
CarriedAbsences AS (
    SELECT
        PersonId,
        FirstTermLateCount AS FirstTermCarriedLateCount,
        FirstTermUnexcusedCount AS FirstTermCarriedUnexcusedCount,
        FirstTermExcusedCount AS FirstTermCarriedExcusedCount,
        SecondTermLateCount AS SecondTermCarriedLateCount,
        SecondTermUnexcusedCount AS SecondTermCarriedUnexcusedCount,
        SecondTermExcusedCount AS SecondTermCarriedExcusedCount,
        (FirstTermLateCount + SecondTermLateCount) AS WholeYearCarriedLateCount,
        (FirstTermUnexcusedCount + SecondTermUnexcusedCount) AS WholeYearCarriedUnexcusedCount,
        (FirstTermExcusedCount + SecondTermExcusedCount) AS WholeYearCarriedExcusedCount
    FROM
        [school_books].[ClassBookStudentCarriedAbsence] ca
    WHERE
        SchoolYear = @schoolYear AND ClassBookId = @classBookId AND PersonId = @personId
)
SELECT
    COALESCE(a.FirstTermLateCount, 0) AS FirstTermLateCount,
    COALESCE(a.FirstTermUnexcusedCount, 0) AS FirstTermUnexcusedCount,
    COALESCE(a.FirstTermExcusedCount, 0) AS FirstTermExcusedCount,
    COALESCE(a.SecondTermLateCount, 0) AS SecondTermLateCount,
    COALESCE(a.SecondTermUnexcusedCount, 0) AS SecondTermUnexcusedCount,
    COALESCE(a.SecondTermExcusedCount, 0) AS SecondTermExcusedCount,
    COALESCE(a.WholeYearLateCount, 0) AS WholeYearLateCount,
    COALESCE(a.WholeYearUnexcusedCount, 0) AS WholeYearUnexcusedCount,
    COALESCE(a.WholeYearExcusedCount, 0) AS WholeYearExcusedCount,
    COALESCE(ca.FirstTermCarriedLateCount, 0) AS FirstTermCarriedLateCount,
    COALESCE(ca.FirstTermCarriedUnexcusedCount, 0) AS FirstTermCarriedUnexcusedCount,
    COALESCE(ca.FirstTermCarriedExcusedCount, 0) AS FirstTermCarriedExcusedCount,
    COALESCE(ca.SecondTermCarriedLateCount, 0) AS SecondTermCarriedLateCount,
    COALESCE(ca.SecondTermCarriedUnexcusedCount, 0) AS SecondTermCarriedUnexcusedCount,
    COALESCE(ca.SecondTermCarriedExcusedCount, 0) AS SecondTermCarriedExcusedCount,
    COALESCE(ca.WholeYearCarriedLateCount, 0) AS WholeYearCarriedLateCount,
    COALESCE(ca.WholeYearCarriedUnexcusedCount, 0) AS WholeYearCarriedUnexcusedCount,
    COALESCE(ca.WholeYearCarriedExcusedCount, 0) AS WholeYearCarriedExcusedCount
FROM
    Absences a
    FULL OUTER JOIN CarriedAbsences ca ON a.PersonId = ca.PersonId
";
        var result = await this.DbContext.Set<GetStudentAbsencesVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("personId", personId))
            .ToArrayAsync(ct);

        return result.SingleOrDefault();
    }

    public async Task<GetStudentAbsencesByDateVO[]> GetAbsencesByDateAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var sql =
$@"
SELECT
    a.Date,
    CAST(SUM(IIF(a.Type = {(int)AbsenceType.Late}, 1, 0)) AS INT) AS LateCount,
    CAST(SUM(IIF(a.Type = {(int)AbsenceType.Unexcused}, 1, 0)) AS INT) AS UnexcusedCount,
    CAST(SUM(IIF(a.Type = {(int)AbsenceType.Excused}, 1, 0)) AS INT) AS ExcusedCount
FROM
    [school_books].[Absence] a
WHERE
    a.SchoolYear = @schoolYear AND
    a.ClassBookId = @classBookId AND
    a.PersonId = @personId
GROUP BY
    a.Date
";
        return await this.DbContext.Set<GetStudentAbsencesByDateVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("personId", personId))
            .ToArrayAsync(ct);
    }

    public async Task<GetStudentGradeResultsVO?> GetGradeResultsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var studentRetakeExams = await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
                on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }

            join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId &&
                gr.PersonId == personId

            select string.Join(" ", s.SubjectName, st.Name)
            )
            .ToArrayAsync(ct);

        return await (
            from gr in this.DbContext.Set<GradeResult>()

            where gr.SchoolYear == schoolYear &&
                  gr.ClassBookId == classBookId &&
                  gr.PersonId == personId

            select new GetStudentGradeResultsVO(
                gr.InitialResultType,
                studentRetakeExams.ToArray()
            )
        ).SingleOrDefaultAsync(ct);
    }

    public async Task<GetStudentGradeResultSessionsVO> GetGradeResultSessionsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var sessions = await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
            on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }

            join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join p in this.DbContext.Set<Person>() on gr.PersonId equals p.PersonId

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId &&
                gr.PersonId == personId

            select new
            {
                c.CurriculumGroupNum,
                gr.FinalResultType,
                SubjectName = string.Join(" / ", s.SubjectName, st.Name),
                grs.Session1NoShow,
                grs.Session1Grade,
                grs.Session2NoShow,
                grs.Session2Grade,
                grs.Session3NoShow,
                grs.Session3Grade,
                })
        .ToArrayAsync(ct);

        if (sessions != null && sessions.Any())
        {
#nullable disable
            return sessions
            .GroupBy(s => s.FinalResultType)
            .Select(g =>
                new GetStudentGradeResultSessionsVO(
                    g.Key,
                    g.OrderBy(gi => gi.CurriculumGroupNum)
                        .Select(gi => new GetStudentGradeResultSessionVO(
                            gi.SubjectName,
                            gi.Session1NoShow,
                            gi.Session1Grade,
                            gi.Session2NoShow,
                            gi.Session2Grade,
                            gi.Session3NoShow,
                            gi.Session3Grade
                    )).ToArray()
                )
            ).SingleOrDefault();
#nullable enable
        }
        else
        {
            return new GetStudentGradeResultSessionsVO(null, Array.Empty<GetStudentGradeResultSessionVO>());
        }
    }

    public async Task<GetStudentFirstGradeResultVO> GetFirstGradeResultAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var result = await (
            from r in this.DbContext.Set<FirstGradeResult>()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.PersonId == personId

            select new GetStudentFirstGradeResultVO(r.QualitativeGrade, r.SpecialGrade))
        .SingleOrDefaultAsync(ct);

        return result ?? new GetStudentFirstGradeResultVO(null, null);
    }

    public async Task<GetStudentSanctionsVO[]> GetSanctionsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Sanction>()
            join st in this.DbContext.Set<SanctionType>() on s.SanctionTypeId equals st.Id
            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.PersonId == personId

            select new GetStudentSanctionsVO(
                st.Name,
                s.OrderNumber,
                s.OrderDate,
                s.CancelOrderNumber,
                s.CancelOrderDate))
            .ToArrayAsync(ct);
    }

    public async Task<GetStudentSupportsVO[]> GetSupportsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var activities = (await (
            from s in this.DbContext.Set<Support>()

            join a in this.DbContext.Set<SupportActivity>()
                on new { s.SchoolYear, s.SupportId }
                equals new { a.SchoolYear, a.SupportId }

            join sat in this.DbContext.Set<SupportActivityType>() on a.SupportActivityTypeId equals sat.Id

            where s.SchoolYear == schoolYear && s.ClassBookId == classBookId

            orderby a.SupportActivityId

            select new
            {
                s.SupportId,
                ActivityTypeName = sat.Name,
                a.Date,
                a.Target,
                a.Result

            })
            .ToArrayAsync(ct))
            .ToLookup(
                a => a.SupportId,
                a => new GetStudentSupportsVOActivity(a.ActivityTypeName, a.Date, a.Target, a.Result));

        var difficulties = (await (
            from s in this.DbContext.Set<Support>()

            join d in this.DbContext.Set<SupportDifficulty>()
                on new { s.SchoolYear, s.SupportId }
                equals new { d.SchoolYear, d.SupportId }

            join sdt in this.DbContext.Set<SupportDifficultyType>() on d.SupportDifficultyTypeId equals sdt.Id

            where s.SchoolYear == schoolYear && s.ClassBookId == classBookId

            select new
            {
                s.SupportId,
                sdt.Name
            })
            .ToArrayAsync(ct))
            .ToLookup(
                d => d.SupportId,
                d => d.Name);

        var teachers = (await (
            from s in this.DbContext.Set<Support>()

            join st in this.DbContext.Set<SupportTeacher>()
                on new { s.SchoolYear, s.SupportId }
                equals new { st.SchoolYear, st.SupportId }

            join p in this.DbContext.Set<Person>() on st.PersonId equals p.PersonId
            where s.SchoolYear == schoolYear && s.ClassBookId == classBookId
            select new
            {
                s.SupportId,
                TeacherName = StringUtils.JoinNames(p.FirstName, p.LastName)
            }
            )
            .ToArrayAsync(ct))
            .ToLookup(
                d => d.SupportId,
                d => d.TeacherName);

        return await (
           from s in this.DbContext.Set<Support>()

           join st in this.DbContext.Set<SupportStudent>()
                on new { s.SchoolYear, s.SupportId }
                equals new { st.SchoolYear, st.SupportId }
                into j0
           from st in j0.DefaultIfEmpty()

           where s.SchoolYear == schoolYear &&
               s.ClassBookId == classBookId &&
               (st.PersonId == personId || s.IsForAllStudents)

           orderby s.SupportId

           select new GetStudentSupportsVO(
               string.Join(", ", difficulties[s.SupportId]),
               s.Description,
               s.ExpectedResult,
               s.EndDate,
               teachers[s.SupportId].ToArray(),
               activities[s.SupportId].ToArray()))
           .ToArrayAsync(ct);
    }

    public async Task<GetStudentNotesVO[]> GetNotesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        return await (
            from n in this.DbContext.Set<Note>()
            join csu in this.DbContext.Set<SysUser>() on n.CreatedBySysUserId equals csu.SysUserId
            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join st in this.DbContext.Set<NoteStudent>()
                on new { n.SchoolYear, n.NoteId }
                equals new { st.SchoolYear, st.NoteId }
                into j1
            from st in j1.DefaultIfEmpty()

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId &&
                (st.PersonId == personId || n.IsForAllStudents)

            orderby n.NoteId

            select new GetStudentNotesVO(
                n.CreateDate,
                StringUtils.JoinNames(cp.FirstName, cp.MiddleName, cp.LastName),
                n.Description))
            .ToArrayAsync(ct);
    }

    public async Task<GetStudentAttendancesVO[]> GetAttendancesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        return await (
                from a in this.DbContext.Set<Attendance>()

                where a.SchoolYear == schoolYear &&
                    a.ClassBookId == classBookId &&
                    a.PersonId == personId

                select new GetStudentAttendancesVO(
                    a.Type,
                    a.Date)
            ).ToArrayAsync(ct);
    }

    public async Task<GetStudentPgResultsVO[]> GetPgResultsAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        return await (
            from pgr in this.DbContext.Set<PgResult>()

            where pgr.SchoolYear == schoolYear &&
                pgr.ClassBookId == classBookId &&
                pgr.PersonId == personId

            select new GetStudentPgResultsVO(
                pgr.StartSchoolYearResult,
                pgr.EndSchoolYearResult)
            ).ToArrayAsync(ct);
    }

    public async Task<GetStudentIndividualWorksVO[]> GetIndividualWorksAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
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

        return await (
            from iw in this.DbContext.Set<IndividualWork>()

            where iw.SchoolYear == schoolYear &&
                iw.ClassBookId == classBookId &&
                iw.PersonId == personId

            select new GetStudentIndividualWorksVO(
                iw.Date,
                iw.IndividualWorkActivity))
            .ToArrayAsync(ct);
    }
}
