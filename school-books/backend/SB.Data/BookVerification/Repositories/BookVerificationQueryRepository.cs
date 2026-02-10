namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IBookVerificationQueryRepository;

internal class BookVerificationQueryRepository : Repository, IBookVerificationQueryRepository
{
    public BookVerificationQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    [Keyless]
    private record GetYearViewQO(
        int Year,
        int Month,
        int LessonsTotal,
        int LessonsTaken,
        int LessonsVerified);
    public async Task<GetYearViewVO[]> GetYearViewAsync(
        int schoolYear,
        int instId,
        int? classBookId,
        int? teacherPersonId,
        CancellationToken ct)
    {
    var sql =
$@"
SELECT
Year = DATEPART(year, sl.Date),
Month = DATEPART(month, sl.Date),
LessonsTotal = COUNT(*),
LessonsTaken = SUM(IIF(t.TopicId IS NOT NULL, 1, 0)),
LessonsVerified = SUM(IIF(sl.IsVerified = 1, 1, 0))
FROM
[school_books].[ClassBook] cb
INNER JOIN [school_books].[Schedule] s ON cb.SchoolYear = s.SchoolYear AND cb.ClassBookId = s.ClassBookId
INNER JOIN [school_books].[ScheduleLesson] sl ON s.SchoolYear = sl.SchoolYear AND s.ScheduleId = sl.ScheduleId
LEFT JOIN [school_books].[ClassBookOffDayDate] odd ON cb.SchoolYear = odd.SchoolYear AND cb.ClassBookId = odd.ClassBookId AND sl.Date = odd.Date
LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND tah.ScheduleLessonId = sl.ScheduleLessonId
LEFT JOIN [school_books].[Topic] t ON sl.SchoolYear = t.SchoolYear AND sl.ScheduleLessonId = t.ScheduleLessonId
{(teacherPersonId != null ? @"
INNER JOIN [inst_year].[CurriculumTeacher] ct ON sl.CurriculumId = ct.CurriculumID
LEFT JOIN [inst_basic].[StaffPosition] sp ON ct.StaffPositionId = sp.StaffPositionId AND sp.PersonId = @teacherPersonId
LEFT JOIN [school_books].[TopicTeacher] tt ON t.SchoolYear = tt.SchoolYear AND t.TopicId = tt.TopicId"
: string.Empty)}
WHERE
cb.SchoolYear = @schoolYear AND
cb.InstId = @instId AND
cb.IsValid = 1 AND
odd.OffDayId IS NULL AND
(tah.TeacherAbsenceId IS NULL OR tah.ReplTeacherPersonId IS NOT NULL) AND

{(classBookId != null ?
    "cb.ClassBookId = @classBookId AND"
    : "1 = 1 AND")}

{(teacherPersonId != null ? @"
    ((t.TopicId IS NOT NULL AND EXISTS (
        SELECT 1 
        FROM [school_books].[TopicTeacher] tt 
        WHERE tt.SchoolYear = t.SchoolYear 
            AND tt.TopicId = t.TopicId 
            AND tt.PersonId = @teacherPersonId)) OR
    (t.TopicId IS NULL AND tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId = @teacherPersonId) OR
    (t.TopicId IS NULL AND tah.TeacherAbsenceId IS NULL AND sp.PersonId IS NOT NULL)) AND
    ((cb.SchoolYear <= CAST(2022 AS SMALLINT)) OR
     (cb.SchoolYear > CAST(2022 AS SMALLINT)
	  AND COALESCE(ct.StaffPositionStartDate, ct.ValidFrom, DATEFROMPARTS(@schoolYear, 9, 01)) <= sl.Date
	  AND ((ct.StaffPositionTerminationDate IS NULL) OR ct.StaffPositionTerminationDate >= sl.Date))) AND"
    : "1 = 1 AND")}
1 = 1
GROUP BY
DATEPART(year, sl.Date),
DATEPART(month, sl.Date)
";

        var parameters = new List<SqlParameter>()
        {
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("instId", instId)
        };

        if (classBookId != null)
        {
            parameters.Add(new SqlParameter("classBookId", classBookId));
        }

        if (teacherPersonId != null)
        {
            parameters.Add(new SqlParameter("teacherPersonId", teacherPersonId));
        }

        var monthStats = await this.DbContext.Set<GetYearViewQO>()
            .FromSqlRaw(sql, parameters.ToArray())
            .ToDictionaryAsync(m => (m.Year, m.Month), ct);

        var hasYearEndSeptemberData = monthStats.ContainsKey((Year: schoolYear + 1, Month: 9));

        var schoolYearStartDateLimit = new DateTime(schoolYear, 9, 1);
        var schoolYearEndDateLimit =
            hasYearEndSeptemberData ? new DateTime(schoolYear + 1, 9, 30) : new DateTime(schoolYear + 1, 8, 31);

        return DateExtensions.GetMonthsInRange(schoolYearStartDateLimit, schoolYearEndDateLimit)
            .Select(m =>
            {
                var ms = monthStats.GetValueOrDefault(m);

                return new GetYearViewVO(
                    m.year,
                    m.month,
                    ms != null,
                    ms != null ? ms.LessonsTaken * 100 / ms.LessonsTotal : 0,
                    ms != null ? ms.LessonsVerified * 100 / ms.LessonsTotal : 0);
            })
            .ToArray();
    }

    [Keyless]
    private record GetMonthViewQO(
        int Day,
        int LessonsTotal,
        int LessonsTaken,
        int LessonsVerified);
    public async Task<GetMonthViewVO[]> GetMonthViewAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int? classBookId,
        int? teacherPersonId,
        CancellationToken ct)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));

    var sql =
$@"
SELECT
Day = DATEPART(day, sl.Date),
LessonsTotal = COUNT(*),
LessonsTaken = SUM(IIF(t.TopicId IS NOT NULL, 1, 0)),
LessonsVerified = SUM(IIF(sl.IsVerified = 1, 1, 0))
FROM
[school_books].[ClassBook] cb
INNER JOIN [school_books].[Schedule] s ON cb.SchoolYear = s.SchoolYear AND cb.ClassBookId = s.ClassBookId
INNER JOIN [school_books].[ScheduleLesson] sl ON s.SchoolYear = sl.SchoolYear AND s.ScheduleId = sl.ScheduleId
LEFT JOIN [school_books].[ClassBookOffDayDate] odd ON cb.SchoolYear = odd.SchoolYear AND cb.ClassBookId = odd.ClassBookId AND sl.Date = odd.Date
LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND tah.ScheduleLessonId = sl.ScheduleLessonId
LEFT JOIN [school_books].[Topic] t ON sl.SchoolYear = t.SchoolYear AND sl.ScheduleLessonId = t.ScheduleLessonId
{(teacherPersonId != null ? @"
INNER JOIN [inst_year].[CurriculumTeacher] ct ON sl.CurriculumId = ct.CurriculumID
LEFT JOIN [inst_basic].[StaffPosition] sp ON ct.StaffPositionId = sp.StaffPositionId AND sp.PersonId = @teacherPersonId
LEFT JOIN [school_books].[TopicTeacher] tt ON t.SchoolYear = tt.SchoolYear AND t.TopicId = tt.TopicId"
: string.Empty)}
WHERE
cb.SchoolYear = @schoolYear AND
cb.InstId = @instId AND
cb.IsValid = 1 AND
odd.OffDayId IS NULL AND
(tah.TeacherAbsenceId IS NULL OR tah.ReplTeacherPersonId IS NOT NULL) AND
sl.Date >= @monthStart AND
sl.Date <= @monthEnd AND

{(classBookId != null ?
    "cb.ClassBookId = @classBookId AND"
    : "1 = 1 AND")}

{(teacherPersonId != null ? @"
    ((t.TopicId IS NOT NULL AND EXISTS (
        SELECT 1 
        FROM [school_books].[TopicTeacher] tt 
        WHERE tt.SchoolYear = t.SchoolYear 
            AND tt.TopicId = t.TopicId 
            AND tt.PersonId = @teacherPersonId)) OR
    (t.TopicId IS NULL AND tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId = @teacherPersonId) OR
    (t.TopicId IS NULL AND tah.TeacherAbsenceId IS NULL AND sp.PersonId IS NOT NULL)) AND
    ((cb.SchoolYear <= CAST(2022 AS SMALLINT)) OR
     (cb.SchoolYear > CAST(2022 AS SMALLINT)
	  AND COALESCE(ct.StaffPositionStartDate, ct.ValidFrom, DATEFROMPARTS(@schoolYear, 9, 01)) <= sl.Date
	  AND ((ct.StaffPositionTerminationDate IS NULL) OR ct.StaffPositionTerminationDate >= sl.Date))) AND"
    : "1 = 1 AND")}
1 = 1
GROUP BY
DATEPART(year, sl.Date),
DATEPART(month, sl.Date),
DATEPART(day, sl.Date)
";

        var parameters = new List<SqlParameter>()
        {
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("instId", instId),
                new SqlParameter("monthStart", monthStart),
                new SqlParameter("monthEnd", monthEnd),
        };

        if (classBookId != null)
        {
            parameters.Add(new SqlParameter("classBookId", classBookId));
        }

        if (teacherPersonId != null)
        {
            parameters.Add(new SqlParameter("teacherPersonId", teacherPersonId));
        }

        var dayStats = await this.DbContext.Set<GetMonthViewQO>()
            .FromSqlRaw(sql, parameters.ToArray())
            .ToDictionaryAsync(d => d.Day, ct);

        var offDays = await this.GetOffDaysForMonthAsync(schoolYear, instId, year, month, classBookId, ct);

        return DateExtensions.GetDatesInRange(monthStart, monthEnd)
            .Select(d =>
            {
                var ds = dayStats.GetValueOrDefault(d.Day);

                return new GetMonthViewVO(
                    d.Day,
                    ds != null,
                    offDays.Contains(d.Day),
                    ds != null ? ds.LessonsTaken * 100 / ds.LessonsTotal : 0,
                    ds != null ? ds.LessonsVerified * 100 / ds.LessonsTotal : 0);
            })
            .ToArray();
    }

    public async Task<GetScheduleLessonsForDayVO[]> GetScheduleLessonsForDayAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int day,
        int? classBookId,
        int? teacherPersonId,
        CancellationToken ct)
    {
        var date = new DateTime(year, month, day);

        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        classBookPredicate = classBookPredicate.And(cb => cb.IsValid);
        if (classBookId != null)
        {
            classBookPredicate = classBookPredicate.And(cb => cb.ClassBookId == classBookId);
        }

        var scheduleLessonIds = await (
            from cb in this.DbContext.Set<ClassBook>().AsNoTracking().Where(classBookPredicate)

            join s in this.DbContext.Set<Schedule>().AsNoTracking()
                on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }
            join sl in this.DbContext.Set<ScheduleLesson>().AsNoTracking()
                on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                sl.Date == date

            select sl.ScheduleLessonId
        )
        .Distinct()
        .ToArrayAsync(ct);

        var absencesCounts = scheduleLessonIds.Length == 0
            ? new Dictionary<int, (int AbsencesCount, int LateAbsencesCount)>()
            : await (
                    from a in this.DbContext.Set<Absence>().AsNoTracking()

                    join id in this.DbContext.MakeIdsQuery(scheduleLessonIds) on a.ScheduleLessonId equals id.Id

                    where a.SchoolYear == schoolYear

                    group a by a.ScheduleLessonId into g

                    select new
                    {
                        ScheduleLessonId = g.Key,
                        AbsencesCount = g.Count(x => x.Type != AbsenceType.Late),
                        LateAbsencesCount = g.Count(x => x.Type == AbsenceType.Late),
                    })
                .ToDictionaryAsync(x => x.ScheduleLessonId, x => (x.AbsencesCount, x.LateAbsencesCount), ct);

        var gradesCounts = await (
            from cb in this.DbContext.Set<ClassBook>().AsNoTracking().Where(classBookPredicate)

            join s in this.DbContext.Set<Schedule>().AsNoTracking()
            on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>().AsNoTracking()
            on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join g in this.DbContext.Set<Grade>().AsNoTracking()
            on new { sl.SchoolYear, ScheduleLessonId = (int?)sl.ScheduleLessonId } equals new { g.SchoolYear, g.ScheduleLessonId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                sl.Date == date

            select new
            {
                sl.ScheduleLessonId,
                g.GradeId
            }
        )
        .GroupBy(sl => sl.ScheduleLessonId)
        .Select(g => new
        {
            ScheduleLessonId = g.Key,
            GradesCount = g.Count(),
        })
        .ToDictionaryAsync(sl => sl.ScheduleLessonId, sl => sl.GradesCount, ct);

        var topicTeachers = (await (
            from cb in this.DbContext.Set<ClassBook>().AsNoTracking().Where(classBookPredicate)

            join s in this.DbContext.Set<Schedule>().AsNoTracking()
            on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }

            join sl in this.DbContext.Set<ScheduleLesson>().AsNoTracking()
            on new { s.SchoolYear, s.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g1 from tah in g1.DefaultIfEmpty()

            join t in this.DbContext.Set<Topic>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { t.SchoolYear, t.ScheduleLessonId }

            join tt in this.DbContext.Set<TopicTeacher>().AsNoTracking()
            on new { sl.SchoolYear, t.TopicId } equals new { tt.SchoolYear, tt.TopicId }

            join p in this.DbContext.Set<Person>().AsNoTracking()
            on tt.PersonId equals p.PersonId

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                (tah == null || tah.ReplTeacherPersonId != null) &&
                sl.Date == date

            select new
            {
                sl.ScheduleLessonId,
                TeacherPersonId = p.PersonId,
                TeacherFirstName = p.FirstName,
                TeacherLastName = p.LastName,
                tt.IsReplTeacher
            }
        ).ToListAsync(ct))
        .ToLookup(
            tt => tt.ScheduleLessonId,
            tt => new GetScheduleLessonsForDayVOTeacher
            {
                TeacherPersonId = tt.TeacherPersonId,
                TeacherFirstName = tt.TeacherFirstName,
                TeacherLastName = tt.TeacherLastName,
                IsReplTeacher = tt.IsReplTeacher
            });

        var scheduleLessons = await (
            from cb in this.DbContext.Set<ClassBook>().AsNoTracking().Where(classBookPredicate)

            join sch in this.DbContext.Set<Schedule>().AsNoTracking()
            on new { cb.SchoolYear, cb.ClassBookId } equals new { sch.SchoolYear, sch.ClassBookId }

            join p in this.DbContext.Set<Person>().AsNoTracking()
            on sch.PersonId equals p.PersonId
            into g1 from p in g1.DefaultIfEmpty()

            join sl in this.DbContext.Set<ScheduleLesson>().AsNoTracking()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join sh in this.DbContext.Set<ShiftHour>().AsNoTracking()
            on new { sch.SchoolYear, sch.ShiftId, sl.Day, sl.HourNumber } equals new { sh.SchoolYear, sh.ShiftId, sh.Day, sh.HourNumber }

            join c in this.DbContext.Set<Curriculum>().AsNoTracking() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>().AsNoTracking() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>().AsNoTracking() on c.SubjectTypeId equals st.SubjectTypeId

            join tah in this.DbContext.Set<TeacherAbsenceHour>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g2 from tah in g2.DefaultIfEmpty()

            join lsh in this.DbContext.Set<LectureScheduleHour>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { lsh.SchoolYear, lsh.ScheduleLessonId }
            into g3
            from lsh in g3.DefaultIfEmpty()

            join t in this.DbContext.Set<Topic>().AsNoTracking()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { t.SchoolYear, t.ScheduleLessonId }
            into g4 from t in g4.DefaultIfEmpty()

            join repl in this.DbContext.Set<Person>().AsNoTracking()
            on new { PersonId = tah.ReplTeacherPersonId } equals new { PersonId = (int?)repl.PersonId }
            into g5 from repl in g5.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                (tah == null || tah.ReplTeacherPersonId != null || !string.IsNullOrEmpty(tah.ExtReplTeacherName)) &&
                sl.Date == date

            orderby cb.BasicClassId, cb.BookName, sl.HourNumber

            select new
            {
                sl.ScheduleLessonId,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime,
                cb.ClassBookId,
                ClassBookFullName = cb.FullBookName,
                ClassBookType = cb.BookType,
                sch.IsIndividualSchedule,
                IsIndividualLesson = (c.IsIndividualLesson ?? 0) != 0,
                IsIndividualCurriculum = c.IsIndividualCurriculum ?? false,
                p.FirstName,
                p.MiddleName,
                p.LastName,

                c.CurriculumId,
                CurriculumGroupName = (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                s.SubjectName,
                s.SubjectNameShort,
                SubjectTypeName = st.Name,

                sl.IsVerified,
                IsTaken = t != null,
                IsLectureScheduleHour = lsh != null,
                TopicTitles = t.Titles.Select(x => x.Title).ToArray(),

                CurriculumTeachers = (
                    from t in this.DbContext.Set<CurriculumTeacher>().AsNoTracking()

                    join sp in this.DbContext.Set<StaffPosition>().AsNoTracking()
                    on t.StaffPositionId equals sp.StaffPositionId

                    join p in this.DbContext.Set<Person>().AsNoTracking()
                    on sp.PersonId equals p.PersonId

                    // Getting only valid teachers for the date of the lesson
                    where t.CurriculumId == sl.CurriculumId &&
                          ((t.SchoolYear <= 2022 && t.IsValid) ||
                          (t.SchoolYear > 2022 &&
                           (t.StaffPositionStartDate ?? t.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= sl.Date &&
                           (t.StaffPositionTerminationDate == null || t.StaffPositionTerminationDate >= sl.Date)))

                    select new GetScheduleLessonsForDayVOTeacher
                    {
                        TeacherPersonId = p.PersonId,
                        TeacherFirstName = p.FirstName,
                        TeacherLastName = p.LastName,
                        IsReplTeacher = false,
                        MarkedAsNoReplacement = t.NoReplacement
                    }
                ).ToArray(),

                ReplTeacher = tah.ReplTeacherPersonId != null ?
                    new GetScheduleLessonsForDayVOTeacher
                    {
                        TeacherPersonId = tah.ReplTeacherPersonId!.Value,
                        TeacherFirstName = repl.FirstName,
                        TeacherLastName = repl.LastName,
                        IsReplTeacher = true,
                        MarkedAsNoReplacement = false
                    }
                    : null,
                ExtReplTeacherName = !string.IsNullOrEmpty(tah.ExtReplTeacherName) ? tah.ExtReplTeacherName + " (Външ. Л.)" : null,
                tah.ReplTeacherIsNonSpecialist,
            }
        )
        .ToListAsync(ct);

        return scheduleLessons.Select(sl => new GetScheduleLessonsForDayVO(
            sl.ScheduleLessonId,
            sl.HourNumber,
            sl.StartTime.ToString(@"hh\:mm"),
            sl.EndTime.ToString(@"hh\:mm"),
            sl.ClassBookId,
            sl.ClassBookFullName,
            sl.ClassBookType,
            sl.IsIndividualSchedule,
            sl.IsIndividualLesson,
            sl.IsIndividualCurriculum,
            sl.FirstName,
            sl.MiddleName,
            sl.LastName,
            sl.CurriculumId,
            sl.CurriculumGroupName,
            sl.SubjectName,
            sl.SubjectNameShort,
            sl.SubjectTypeName,
            sl.ReplTeacherIsNonSpecialist,
            topicTeachers[sl.ScheduleLessonId].Any()
                ? topicTeachers[sl.ScheduleLessonId].ToArray()
                : sl.ReplTeacher != null
                    ? new [] { sl.ReplTeacher }
                    : sl.CurriculumTeachers,
            sl.ExtReplTeacherName,
            sl.IsLectureScheduleHour,
            sl.IsTaken,
            sl.TopicTitles,
            absencesCounts.GetValueOrDefault(sl.ScheduleLessonId).AbsencesCount,
            absencesCounts.GetValueOrDefault(sl.ScheduleLessonId).LateAbsencesCount,
            gradesCounts.GetValueOrDefault(sl.ScheduleLessonId),
            sl.IsVerified
        ))
        .Where(sl => teacherPersonId == null || sl.Teachers.Any(t => t.TeacherPersonId == teacherPersonId) || !string.IsNullOrEmpty(sl.ExtReplTeacherName))
        .ToArray();
    }

    public async Task<bool> ExistTopicsAsync(
        int schoolYear,
        int[] scheduleLessonIds,
        CancellationToken ct)
    {
        var uniqueIdsCount = scheduleLessonIds.Distinct().Count();
        var topicsCount = await this.DbContext.Set<Topic>()
            .Where(t =>
                t.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(scheduleLessonIds)
                    .Any(id => t.ScheduleLessonId == id.Id))
            .CountAsync(ct);

        return uniqueIdsCount == topicsCount;
    }

    public async Task<GetOffDayVO[]> GetOffDaysForDayAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int day,
        int? classBookId,
        CancellationToken ct)
    {
        var date = new DateTime(year, month, day);

        var predicate = PredicateBuilder.True<ClassBookOffDayDate>();
        if (classBookId != null)
        {
            predicate = predicate.And(cb => cb.ClassBookId == classBookId);
        }

        return await (
            from cbodd in this.DbContext.Set<ClassBookOffDayDate>().Where(predicate)
            join od in this.DbContext.Set<OffDay>() on new { cbodd.SchoolYear, cbodd.OffDayId } equals new { od.SchoolYear, od.OffDayId }

            where od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                cbodd.Date == date

            select new GetOffDayVO(
                cbodd.ClassBookId,
                od.From,
                od.To,
                od.Description))
            .ToArrayAsync(ct);
    }

    private async Task<int[]> GetOffDaysForMonthAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int? classBookId,
        CancellationToken ct)
    {
        int classBooksCount;
        IQueryable<int> offDays;

        if (classBookId != null)
        {
            classBooksCount = 1;

            offDays =
                from cbodd in this.DbContext.Set<ClassBookOffDayDate>()
                where cbodd.SchoolYear == schoolYear &&
                    cbodd.ClassBookId == classBookId &&
                    cbodd.Date.Year == year &&
                    cbodd.Date.Month == month
                select cbodd.Date.Day;
        }
        else
        {
            classBooksCount = await (
                from cb in this.DbContext.Set<ClassBook>()
                where cb.InstId == instId && cb.SchoolYear == schoolYear
                select cb.ClassBookId)
           .CountAsync(ct);

            offDays =
                from cbodd in this.DbContext.Set<ClassBookOffDayDate>()
                join od in this.DbContext.Set<OffDay>() on new { cbodd.SchoolYear, cbodd.OffDayId } equals new { od.SchoolYear, od.OffDayId }
                where od.SchoolYear == schoolYear &&
                    od.InstId == instId &&
                    cbodd.Date.Year == year &&
                    cbodd.Date.Month == month
                select cbodd.Date.Day;
        }

        return await offDays.GroupBy(odd => odd)
            .Select(g =>
                new
                {
                    Day = g.Key,
                    Count = g.Count(),
                })
            .Where(odd => odd.Count == classBooksCount)
            .Select(odd => odd.Day)
            .ToArrayAsync(ct);
    }
}
