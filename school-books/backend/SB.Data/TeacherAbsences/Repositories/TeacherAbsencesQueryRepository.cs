namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ITeacherAbsencesQueryRepository;

internal class TeacherAbsencesQueryRepository : Repository, ITeacherAbsencesQueryRepository
{
    public TeacherAbsencesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        string? teacherName,
        string? replTeacherName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var teacherPredicate = PredicateBuilder.True<Person>();
        if (!string.IsNullOrWhiteSpace(teacherName))
        {
            string[] words = teacherName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                teacherPredicate = teacherPredicate.AndAnyStringContains(p => p.FirstName, p => p.LastName, word);
            }
        }

        var replTeacherPredicate = PredicateBuilder.True<Person>();
        if (!string.IsNullOrWhiteSpace(replTeacherName))
        {
            string[] words = replTeacherName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                replTeacherPredicate = replTeacherPredicate.AndAnyStringContains(p => p.FirstName, p => p.LastName, word);
            }
        }

        return await this.GetAllAsync(
            schoolYear,
            instId,
            teacherPredicate,
            replTeacherPredicate,
            offset,
            limit,
            ct);
    }

    public async Task<TableResultVO<GetAllVO>> GetAllForAbsenteePersonAsync(
        int schoolYear,
        int instId,
        int personId,
        string? replTeacherName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var teacherPredicate = PredicateBuilder.True<Person>().And(p => p.PersonId == personId);

        var replTeacherPredicate = PredicateBuilder.True<Person>();
        if (!string.IsNullOrWhiteSpace(replTeacherName))
        {
            string[] words = replTeacherName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                replTeacherPredicate = replTeacherPredicate.AndAnyStringContains(p => p.FirstName, p => p.LastName, word);
            }
        }

        return await this.GetAllAsync(
            schoolYear,
            instId,
            teacherPredicate,
            replTeacherPredicate,
            offset,
            limit,
            ct);
    }

    public async Task<TableResultVO<GetAllVO>> GetAllForReplacementPersonAsync(
        int schoolYear,
        int instId,
        int personId,
        string? teacherName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var teacherPredicate = PredicateBuilder.True<Person>();
        if (!string.IsNullOrWhiteSpace(teacherName))
        {
            string[] words = teacherName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                teacherPredicate = teacherPredicate.AndAnyStringContains(p => p.FirstName, p => p.LastName, word);
            }
        }

        var replTeacherPredicate = PredicateBuilder.True<Person>().And(p => p.PersonId == personId);

        return await this.GetAllAsync(
            schoolYear,
            instId,
            teacherPredicate,
            replTeacherPredicate,
            offset,
            limit,
            ct);
    }

    private record TeacherAbsenceResultVO(
        int TeacherAbsenceId,
        int TeacherPersonId,
        string TeacherName,
        DateTime StartDate,
        DateTime EndDate,
        string Reason);
    private async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        Expression<Func<Person, bool>> teacherPredicate,
        Expression<Func<Person, bool>> replTeacherPredicate,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        IQueryable<TeacherAbsenceResultVO> query;

        if (replTeacherPredicate.IsTrueLambdaExpr())
        {
            query = (
                from ta in this.DbContext.Set<TeacherAbsence>()
                join t in this.DbContext.Set<Person>().Where(teacherPredicate) on ta.TeacherPersonId equals t.PersonId

                where ta.SchoolYear == schoolYear &&
                    ta.InstId == instId

                orderby ta.StartDate descending

                select new TeacherAbsenceResultVO(
                    ta.TeacherAbsenceId,
                    ta.TeacherPersonId,
                    StringUtils.JoinNames(t.FirstName, t.LastName),
                    ta.StartDate,
                    ta.EndDate,
                    ta.Reason)
            );
        }
        else
        {
            query = (
                from ta in this.DbContext.Set<TeacherAbsence>()
                join t in this.DbContext.Set<Person>().Where(teacherPredicate) on ta.TeacherPersonId equals t.PersonId

                where ta.SchoolYear == schoolYear &&
                    ta.InstId == instId &&
                    (from tah in this.DbContext.Set<TeacherAbsenceHour>()
                    join rt in this.DbContext.Set<Person>().Where(replTeacherPredicate) on tah.ReplTeacherPersonId equals rt.PersonId
                    where tah.SchoolYear == ta.SchoolYear &&
                        tah.TeacherAbsenceId == ta.TeacherAbsenceId
                    select 1).Any()

                orderby ta.StartDate descending

                select new TeacherAbsenceResultVO(
                    ta.TeacherAbsenceId,
                    ta.TeacherPersonId,
                    StringUtils.JoinNames(t.FirstName, t.LastName),
                    ta.StartDate,
                    ta.EndDate,
                    ta.Reason)
            );
        }

        int length = await query.CountAsync(ct);
        if (length == 0)
        {
            return TableResultVO.Empty<GetAllVO>();
        }

        var result = await query
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);

        var teacherAbsenceIds = result.Select(c => c.TeacherAbsenceId).ToArray();
        var idSet = teacherAbsenceIds.ToHashSet();

        var relatedReplTeacherNames =
            (await (
                from tah in this.DbContext.Set<TeacherAbsenceHour>()
                join rt in this.DbContext.Set<Person>() on tah.ReplTeacherPersonId equals rt.PersonId
                where idSet.Contains(tah.TeacherAbsenceId)
                select new
                {
                    tah.TeacherAbsenceId,
                    ReplTeacherName = StringUtils.JoinNames(rt.FirstName, rt.LastName)
                }
            ).Distinct().ToArrayAsync(ct))
            .ToLookup(c => c.TeacherAbsenceId, c => c.ReplTeacherName);

        var relatedExtReplTeacherNames =
            (await (
                from tah in this.DbContext.Set<TeacherAbsenceHour>()
                where idSet.Contains(tah.TeacherAbsenceId) && !string.IsNullOrEmpty(tah.ExtReplTeacherName)
                select new
                {
                    tah.TeacherAbsenceId,
                    tah.ExtReplTeacherName
                }
            ).Distinct().ToArrayAsync(ct))
            .ToLookup(c => c.TeacherAbsenceId, c => c.ExtReplTeacherName + " (Външ. Л.)");

        return new TableResultVO<GetAllVO>(
            result
                .Select(
                    r => new GetAllVO(
                        r.TeacherAbsenceId,
                        r.TeacherPersonId,
                        r.TeacherName,
                        relatedReplTeacherNames[r.TeacherAbsenceId].Concat(relatedExtReplTeacherNames[r.TeacherAbsenceId]).ToArray(),
                        r.StartDate,
                        r.EndDate,
                        r.Reason))
                .ToArray(),
            length);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct)
    {
        string sql = """
            SELECT
                tah.ScheduleLessonId,
                tah.ReplTeacherPersonId,
                ReplTeacherFirstName = repl.FirstName,
                ReplTeacherLastName = repl.LastName,
                tah.ReplTeacherIsNonSpecialist,
                tah.ExtReplTeacherName,
                IsInUse = IIF(
                    EXISTS(
                        SELECT 1
                        FROM [school_books].[Absence] a
                        WHERE
                            a.[SchoolYear] = tah.[SchoolYear] AND
                            a.[ScheduleLessonId] = tah.[ScheduleLessonId] AND
                            a.[TeacherAbsenceId] = tah.[TeacherAbsenceId]
                    ) OR
                    EXISTS(
                        SELECT 1
                        FROM [school_books].[Grade] g
                        WHERE
                            g.[SchoolYear] = tah.[SchoolYear] AND
                            g.[ScheduleLessonId] = tah.[ScheduleLessonId] AND
                            g.[TeacherAbsenceId] = tah.[TeacherAbsenceId]
                    ) OR
                    EXISTS(
                        SELECT 1
                        FROM [school_books].[Topic] t
                        WHERE
                            t.[SchoolYear] = tah.[SchoolYear] AND
                            t.[ScheduleLessonId] = tah.[ScheduleLessonId] AND
                            t.[TeacherAbsenceId] = tah.[TeacherAbsenceId]
                    ),
                    CAST(1 AS BIT),
                    CAST(0 AS BIT)
                )
            FROM
                [school_books].[TeacherAbsenceHour] tah
                LEFT JOIN [school_books].[ScheduleLesson] sl ON tah.SchoolYear = sl.SchoolYear AND tah.ScheduleLessonId = sl.ScheduleLessonId
                LEFT JOIN [core].[Person] repl ON tah.ReplTeacherPersonId = repl.PersonId
            WHERE
                tah.SchoolYear = @schoolYear AND
                tah.TeacherAbsenceId = @teacherAbsenceId
            ORDER BY sl.Date
            """;
        var teacherAbsenceHours = await this.DbContext.Set<GetVOHour>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("teacherAbsenceId", teacherAbsenceId))
            .ToArrayAsync(ct);

        return await (
            from ta in this.DbContext.Set<TeacherAbsence>()

            where ta.SchoolYear == schoolYear &&
                ta.InstId == instId &&
                ta.TeacherAbsenceId == teacherAbsenceId

            select new GetVO(
                ta.TeacherPersonId,
                ta.StartDate,
                ta.EndDate,
                ta.Reason,
                teacherAbsenceHours)
        ).SingleAsync(ct);
    }

    public async Task<bool> ContainsTeacherAsync(
        int schoolYear,
        int teacherAbsenceId,
        int personId,
        CancellationToken ct)
    {
        var teacherAbsence =
            this.DbContext
            .Set<TeacherAbsence>()
            .Where(ta =>
                ta.SchoolYear == schoolYear &&
                ta.TeacherAbsenceId == teacherAbsenceId &&
                ta.TeacherPersonId == personId)
            .Select(ta => ta.TeacherAbsenceId);

        var teacherAbsenceHours =
            this.DbContext
            .Set<TeacherAbsenceHour>()
            .Where(tah =>
                tah.SchoolYear == schoolYear &&
                tah.TeacherAbsenceId == teacherAbsenceId &&
                tah.ReplTeacherPersonId == personId)
            .Select(tah => tah.TeacherAbsenceId);


        return await teacherAbsence.Concat(teacherAbsenceHours).AnyAsync(ct);
    }

    public async Task<GetLessonsVO[]> GetLessonsAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        CancellationToken ct)
    {
        return await (
            from sl in this.DbContext.Set<ScheduleLesson>()
            join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
            join cb in this.DbContext.Set<ClassBook>() on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join tah in this.DbContext.Set<TeacherAbsenceHour>() on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
            into g1 from tah in g1.DefaultIfEmpty()

            where sl.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                this.DbContext
                .MakeIdsQuery(scheduleLessonIds)
                .Any(id => sl.ScheduleLessonId == id.Id)

            select new GetLessonsVO
            {
                ClassBookId = cb.ClassBookId,
                ScheduleLessonId = sl.ScheduleLessonId,
                Date = sl.Date,
                CurriculumTeacherPersonIds = (
                    from t in this.DbContext.Set<CurriculumTeacher>()
                    join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

                    where t.CurriculumId == sl.CurriculumId

                    select
                        sp.PersonId
                ).ToArray(),
                TeacherAbsenceId = tah.TeacherAbsenceId
            }
        ).ToArrayAsync(ct);
    }

    public async Task<GetLessonsInUseVO[]> GetLessonsInUseAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        int? exceptTeacherAbsenceId,
        CancellationToken ct)
    {
        var usedScheduleLessonIds =
            Enumerable.Empty<object>()
            .Select(x =>
                new
                {
                    SchoolYear = default(int),
                    ScheduleLessonId = default(int)
                })
            .AsQueryable();

        if (exceptTeacherAbsenceId.HasValue)
        {
            usedScheduleLessonIds =
                (
                    from a in this.DbContext.Set<Absence>()

                    join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { a.SchoolYear, a.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
                    into g1
                    from tah in g1.DefaultIfEmpty()

                    where tah.TeacherAbsenceId != exceptTeacherAbsenceId.Value

                    select new
                    {
                        a.SchoolYear,
                        a.ScheduleLessonId
                    }
                )
                .Union(
                    from g in this.DbContext.Set<Grade>()

                    join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { g.SchoolYear, g.ScheduleLessonId } equals new { tah.SchoolYear, ScheduleLessonId = (int?)tah.ScheduleLessonId }
                    into g1
                    from tah in g1.DefaultIfEmpty()

                    where tah.TeacherAbsenceId != exceptTeacherAbsenceId.Value

                    select new
                    {
                        g.SchoolYear,
                        ScheduleLessonId = g.ScheduleLessonId!.Value
                    }
                )
                .Union(
                    from t in this.DbContext.Set<Topic>()

                    join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { t.SchoolYear, t.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
                    into g1
                    from tah in g1.DefaultIfEmpty()

                    where tah.TeacherAbsenceId != exceptTeacherAbsenceId.Value

                    select new
                    {
                        t.SchoolYear,
                        t.ScheduleLessonId
                    }
                )
                .Union(
                    from tah in this.DbContext.Set<TeacherAbsenceHour>()

                    where tah.TeacherAbsenceId != exceptTeacherAbsenceId.Value

                    select new
                    {
                        tah.SchoolYear,
                        tah.ScheduleLessonId
                    }
                );
        }
        else
        {
            usedScheduleLessonIds =
                (
                    from a in this.DbContext.Set<Absence>()
                    select new
                    {
                        a.SchoolYear,
                        a.ScheduleLessonId
                    }
                )
                .Union(
                    from g in this.DbContext.Set<Grade>()
                    select new
                    {
                        g.SchoolYear,
                        ScheduleLessonId = g.ScheduleLessonId!.Value
                    }
                )
                .Union(
                    from t in this.DbContext.Set<Topic>()
                    select new
                    {
                        t.SchoolYear,
                        t.ScheduleLessonId
                    }
                )
                .Union(
                    from tah in this.DbContext.Set<TeacherAbsenceHour>()
                    select new
                    {
                        tah.SchoolYear,
                        tah.ScheduleLessonId
                    }
                );
        }

        return await (
            from usl in usedScheduleLessonIds
            join sl in this.DbContext.Set<ScheduleLesson>() on usl equals new { sl.SchoolYear, sl.ScheduleLessonId }
            join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
            join cb in this.DbContext.Set<ClassBook>() on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, cb.ClassId } equals new { cg.SchoolYear, cg.ClassId }

            where cb.IsValid && usl.SchoolYear == schoolYear &&
                this.DbContext
                .MakeIdsQuery(scheduleLessonIds)
                .Any(id => usl.ScheduleLessonId == id.Id)

            select new GetLessonsInUseVO(
                sl.ScheduleLessonId,
                sl.Date,
                sl.Day,
                sl.HourNumber,
                cg.ClassName)
        ).ToArrayAsync(ct);
    }

    public async Task<GetTeacherAbsenceHoursInUseVO[]> GetTeacherAbsenceHoursInUseAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct)
    {
        var usedTeacherAbsenceHourIds =
            (
                from a in this.DbContext.Set<Absence>()
                join cb in this.DbContext.Set<ClassBook>() on new { a.SchoolYear, a.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { a.SchoolYear, a.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
                    into g1
                from tah in g1.DefaultIfEmpty()

                where cb.SchoolYear == schoolYear &&
                    cb.InstId == instId &&
                    cb.IsValid &&
                    tah.TeacherAbsenceId == teacherAbsenceId

                select new
                {
                    a.SchoolYear,
                    TeacherAbsenceId = a.TeacherAbsenceId!.Value,
                    a.ScheduleLessonId
                }
            )
            .Union(
                from g in this.DbContext.Set<Grade>()
                join cb in this.DbContext.Set<ClassBook>() on new { g.SchoolYear, g.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { g.SchoolYear, g.ScheduleLessonId } equals new { tah.SchoolYear, ScheduleLessonId = (int?)tah.ScheduleLessonId }
                    into g1
                from tah in g1.DefaultIfEmpty()

                where
                    cb.SchoolYear == schoolYear &&
                    cb.InstId == instId &&
                    cb.IsValid &&
                    tah.TeacherAbsenceId == teacherAbsenceId &&
                    g.ScheduleLessonId != null

                select new
                {
                    g.SchoolYear,
                    TeacherAbsenceId = g.TeacherAbsenceId!.Value,
                    ScheduleLessonId = g.ScheduleLessonId!.Value
                }
            )
            .Union(
                from t in this.DbContext.Set<Topic>()
                join cb in this.DbContext.Set<ClassBook>() on new { t.SchoolYear, t.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
                join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { t.SchoolYear, t.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }
                    into g1
                from tah in g1.DefaultIfEmpty()

                where
                    cb.SchoolYear == schoolYear &&
                    cb.InstId == instId &&
                    cb.IsValid &&
                    tah.TeacherAbsenceId == teacherAbsenceId

                select new
                {
                    t.SchoolYear,
                    TeacherAbsenceId = t.TeacherAbsenceId!.Value,
                    t.ScheduleLessonId
                }
            );

        return await (
            from utah in usedTeacherAbsenceHourIds
            join tah in this.DbContext.Set<TeacherAbsenceHour>() on utah equals new { tah.SchoolYear, tah.TeacherAbsenceId, tah.ScheduleLessonId }
            join sl in this.DbContext.Set<ScheduleLesson>() on new { tah.SchoolYear, tah.ScheduleLessonId} equals new { sl.SchoolYear, sl.ScheduleLessonId }
            join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
            join cb in this.DbContext.Set<ClassBook>() on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, cb.ClassId } equals new { cg.SchoolYear, cg.ClassId }

            select new GetTeacherAbsenceHoursInUseVO(
                tah.TeacherAbsenceId,
                tah.ScheduleLessonId,
                tah.ReplTeacherPersonId,
                tah.ReplTeacherIsNonSpecialist,
                sl.Date,
                sl.Day,
                sl.HourNumber,
                cg.ClassName)
        ).ToArrayAsync(ct);
    }

    public async Task<GetScheduleVO> GetScheduleAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct)
    {
        return await this.GetScheduleInternalAsync(
            schoolYear,
            instId,
            h => h.TeacherAbsenceId == teacherAbsenceId,
            h => true,
            ct);
    }

    public async Task<GetScheduleVO> GetTeacherScheduleForPeriodAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct)
    {
        var offDays = (await (
            from odd in this.DbContext.Set<ClassBookOffDayDate>()
            join od in this.DbContext.Set<OffDay>() on new { odd.SchoolYear, odd.OffDayId } equals new { od.SchoolYear, od.OffDayId }
            where
                od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                odd.Date >= startDate &&
                odd.Date <= endDate

            select new
            {
                odd.ClassBookId,
                odd.Date
            }
        )
        .ToArrayAsync(ct))
        .Select(a => (a.ClassBookId, a.Date))
        .ToHashSet();

        var schedule = await this.GetScheduleInternalAsync(
            schoolYear,
            instId,
            h =>
                ((h.TeacherAbsenceId == null && h.CurriculumTeachers.Any(t => t.TeacherPersonId == teacherPersonId)) ||
                 (h.TeacherAbsenceId != null && h.ReplTeacher!.TeacherPersonId == teacherPersonId)) &&
                h.Date >= startDate.Date &&
                h.Date <= endDate.Date,
            h => !offDays.Contains((h.ClassBookId, h.Date)),
            ct);

        return schedule with {
            Slots = schedule.Slots.Select(s => s with {
                Hours = s.Hours
                    .Select(h => h with {
                        IsReplHour = h.TeacherAbsenceId != null && h.ReplTeacher?.TeacherPersonId == teacherPersonId
                }).ToArray()
            }).ToArray()
        };
    }

    public async Task<GetScheduleVO> GetTeacherScheduleForAbsenceAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct)
    {
        var teacherAbsence = await this.DbContext.Set<TeacherAbsence>()
            .SingleAsync(t => t.SchoolYear == schoolYear && t.InstId == instId && t.TeacherAbsenceId == teacherAbsenceId, ct);

        var offDays = (await (
            from odd in this.DbContext.Set<ClassBookOffDayDate>()
            join od in this.DbContext.Set<OffDay>() on new { odd.SchoolYear, odd.OffDayId } equals new { od.SchoolYear, od.OffDayId }
            where
                od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                odd.Date >= teacherAbsence.StartDate &&
                odd.Date <= teacherAbsence.EndDate

            select new
            {
                odd.ClassBookId,
                odd.Date
            }
        )
        .ToArrayAsync(ct))
        .Select(a => (a.ClassBookId, a.Date))
        .ToHashSet();

        var schedule = await this.GetScheduleInternalAsync(
            schoolYear,
            instId,
            h =>
                h.TeacherAbsenceId == teacherAbsenceId ||
                (((h.TeacherAbsenceId == null && h.CurriculumTeachers.Any(t => t.TeacherPersonId == teacherAbsence.TeacherPersonId)) ||
                  (h.TeacherAbsenceId != null && h.ReplTeacher!.TeacherPersonId == teacherAbsence.TeacherPersonId)) &&
                 h.Date >= teacherAbsence.StartDate &&
                 h.Date <= teacherAbsence.EndDate),
            h => h.TeacherAbsenceId == teacherAbsenceId || !offDays.Contains((h.ClassBookId, h.Date)),
            ct);

        return schedule with
        {
            Slots = schedule.Slots.Select(s => s with
            {
                Hours = s.Hours
                    .Select(h => h with
                    {
                        IsReplHour = h.TeacherAbsenceId != null && h.ReplTeacher?.TeacherPersonId == teacherAbsence.TeacherPersonId
                    }).ToArray()
            }).ToArray()
        };
    }

    private async Task<GetScheduleVO> GetScheduleInternalAsync(
        int schoolYear,
        int instId,
        Expression<Func<SchedulesQueryHelper.GetScheduleHoursVO, bool>> scheduleHourPredicate,
        Func<SchedulesQueryHelper.GetScheduleTableVOSlotHour, bool> scheduleTableSlotHourPredicate,
        CancellationToken ct)
    {
        var scheduleTable =
            await this.DbContext.GetScheduleTableAsync(
                includeUsedHoursInfo: true,
                schoolYear,
                cb => cb.InstId == instId,
                s => true,
                scheduleHourPredicate,
                ct: ct);

        return new GetScheduleVO(
            ShiftHours:
                scheduleTable.ShiftHours
                .Select(h =>
                    new GetScheduleVOShiftHour(
                        SlotNumber: h.SlotNumber,
                        StartTime: h.StartTime,
                        EndTime: h.EndTime))
                .ToArray(),
            Slots:
                scheduleTable.Slots
                .Select(sl =>
                    new GetScheduleVOSlot(
                        Day: sl.Day,
                        SlotNumber: sl.SlotNumber,
                        Hours: sl.Hours.Where(scheduleTableSlotHourPredicate)
                            .OrderBy(h => h.Date)
                            .Select(h => new GetScheduleVOSlotHour(
                                ScheduleLessonId: h.ScheduleLessonId,
                                ClassBookId: h.ClassBookId,
                                ClassBookFullName: h.ClassBookFullName,
                                IsClassBookValid: h.IsClassBookValid,
                                Date: h.Date,
                                CurriculumId: h.CurriculumId,
                                CurriculumGroupName: h.CurriculumGroupName,
                                SubjectName: h.SubjectName,
                                SubjectNameShort: h.SubjectNameShort,
                                SubjectTypeName: h.SubjectTypeName,
                                IsIndividualLesson: h.IsIndividualLesson,
                                IsIndividualCurriculum: h.IsIndividualCurriculum,
                                IsIndividualSchedule: h.IsIndividualSchedule,
                                StudentPersonId: h.StudentPersonId,
                                StudentFirstName: h.StudentFirstName,
                                StudentMiddleName: h.StudentMiddleName,
                                StudentLastName: h.StudentLastName,
                                CurriculumTeachers: h.CurriculumTeachers
                                    .Select(t => new GetScheduleVOSlotHourTeacher(
                                        t.TeacherPersonId,
                                        t.TeacherFirstName,
                                        t.TeacherLastName,
                                        t.MarkedAsNoReplacement))
                                    .ToArray(),
                                TeacherAbsenceId: h.TeacherAbsenceId,
                                IsReplHour: false,
                                ReplTeacher: h.ReplTeacher == null
                                    ? null
                                    : new GetScheduleVOSlotHourTeacher(
                                        h.ReplTeacher.TeacherPersonId,
                                        h.ReplTeacher.TeacherFirstName,
                                        h.ReplTeacher.TeacherLastName,
                                        false),
                                ReplTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                                IsEmptyHour: h.IsEmptyHour,
                                IsInUse: h.IsInUse))
                            .ToArray()
                    ))
                .ToArray()
        );
    }

    public async Task<(int ClassBookId, DateTime Date)[]> GetOffDayDatesForClassBooksAsync(
        int schoolYear,
        int instId,
        DateTime from,
        DateTime to,
        int[] classBookIds,
        CancellationToken ct)
    {
        var result = await (
            from odd in this.DbContext.Set<ClassBookOffDayDate>()
            join od in this.DbContext.Set<OffDay>() on new { odd.SchoolYear, odd.OffDayId } equals new { od.SchoolYear, od.OffDayId }

            where od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                @from <= od.To && to >= od.From && // date range overlap
                this.DbContext
                    .MakeIdsQuery(classBookIds)
                    .Any(id => odd.ClassBookId == id.Id)

            select new { odd.ClassBookId, odd.Date }
        ).ToArrayAsync(ct);

        return result.Select(x => (x.ClassBookId, x.Date)).ToArray();
    }

    public async Task<bool> HasInvalidClassBooksForTeacherAbsenceAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct)
        => await (
            from tah in this.DbContext.Set<TeacherAbsenceHour>()

            join sl in this.DbContext.Set<ScheduleLesson>()
                on new { tah.SchoolYear, tah.ScheduleLessonId }
                equals new { sl.SchoolYear, sl.ScheduleLessonId }

            join s in this.DbContext.Set<Schedule>()
                on new { sl.SchoolYear, sl.ScheduleId }
                equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>()
                on new { s.SchoolYear, s.ClassBookId }
                equals new { cb.SchoolYear, cb.ClassBookId }

            where tah.SchoolYear == schoolYear &&
                  tah.TeacherAbsenceId == teacherAbsenceId &&
                  cb.InstId == instId &&
                  !cb.IsValid

            select tah.TeacherAbsenceId
        ).AnyAsync(ct);

}
