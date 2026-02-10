namespace SB.Data;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISchedulesQueryRepository;

internal class SchedulesQueryRepository : Repository, ISchedulesQueryRepository
{
    public SchedulesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetClassBookScheduleForWeekVO> GetClassBookScheduleForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        int? personId,
        CancellationToken ct)
    {
        DateTime startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        DateTime endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        var schedulePredicate = PredicateBuilder.True<Schedule>();
        schedulePredicate =
            personId.HasValue ?
                schedulePredicate.And(s => s.IsIndividualSchedule && s.PersonId == personId) :
                schedulePredicate.And(s => !s.IsIndividualSchedule);

        var scheduleHours = await this.DbContext
            .GetScheduleHours(
                includeUsedHoursInfo: false,
                schoolYear,
                cb => cb.ClassBookId == classBookId,
                schedulePredicate)
            .Where(sh => sh.Date >= startDate && sh.Date <= endDate)
            .ToArrayAsync(ct);

        var offDays = await (
            from od in this.DbContext.Set<ClassBookOffDayDate>()

            where
                od.SchoolYear == schoolYear &&
                od.ClassBookId == classBookId &&
                od.Date >= startDate &&
                od.Date <= endDate

            select
                DateExtensions.GetIsoDayOfWeek(od.Date)
        ).ToArrayAsync(ct);

        return new GetClassBookScheduleForWeekVO(
            scheduleHours
                .Select(h => new GetClassBookScheduleForWeekVOHour(
                    ScheduleLessonId: h.ScheduleLessonId,
                    Date: h.Date,
                    Day: h.Day,
                    HourNumber: h.HourNumber,
                    CurriculumId: h.CurriculumId,
                    CurriculumGroupName: h.CurriculumGroupName,
                    SubjectName: h.SubjectName,
                    SubjectNameShort: h.SubjectNameShort,
                    SubjectTypeName: h.SubjectTypeName,
                    IsIndividualLesson: h.IsIndividualLesson,
                    IsIndividualCurriculum: h.IsIndividualCurriculum,
                    CurriculumTeachers: h.CurriculumTeachers
                        .Select(t => new GetClassBookScheduleForWeekVOHourTeacher(
                            t.TeacherPersonId,
                            t.TeacherFirstName,
                            t.TeacherLastName,
                            t.MarkedAsNoReplacement))
                        .ToArray(),
                    TeacherAbsenceId: h.TeacherAbsenceId,
                    ReplTeacher: h.ReplTeacher == null
                        ? null
                        : new GetClassBookScheduleForWeekVOHourTeacher(
                            h.ReplTeacher.TeacherPersonId,
                            h.ReplTeacher.TeacherFirstName,
                            h.ReplTeacher.TeacherLastName,
                            false),
                    ReplTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                    ExtTeacherName: h.ExtTeacherName,
                    IsEmptyHour: h.IsEmptyHour
                ))
                .ToArray(),
            offDays);
    }

    public async Task<GetClassBookScheduleTableForWeekVO> GetClassBookScheduleTableForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        bool showIndividualCurriculum,
        int? personId,
        bool showOnlyStudentCurriculums = false,
        CancellationToken ct = default)
    {
        var startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        var endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        var schedulePredicate = PredicateBuilder.True<Schedule>();
        schedulePredicate = showIndividualCurriculum ?
            schedulePredicate.And(s => s.IsIndividualSchedule && s.PersonId == personId) :
            schedulePredicate.And(s => !s.IsIndividualSchedule);

        Expression<Func<CurriculumStudent, bool>>? curriculumStudentPredicate = null;
        if (showOnlyStudentCurriculums && personId.HasValue)
        {
            curriculumStudentPredicate = PredicateBuilder.True<CurriculumStudent>().And(cs => cs.PersonId == personId && cs.IsValid);
        }

        var scheduleTable =
            await this.DbContext.GetScheduleTableAsync(
                includeUsedHoursInfo: false,
                schoolYear,
                cb => cb.ClassBookId == classBookId,
                schedulePredicate,
                sh => sh.Date >= startDate && sh.Date <= endDate,
                curriculumStudentPredicate,
                ct);

        var offDays = await (
            from od in this.DbContext.Set<ClassBookOffDayDate>()

            where
                od.SchoolYear == schoolYear &&
                od.ClassBookId == classBookId &&
                od.Date >= startDate &&
                od.Date <= endDate

            select
                DateExtensions.GetIsoDayOfWeek(od.Date)
        ).ToArrayAsync(ct);

        return new GetClassBookScheduleTableForWeekVO(
            ShiftHours:
                scheduleTable.ShiftHours
                .Select(h =>
                    new GetClassBookScheduleTableForWeekVOShiftHour(
                        SlotNumber: h.SlotNumber,
                        StartTime: h.StartTime,
                        EndTime: h.EndTime))
                .ToArray(),
            Slots:
                scheduleTable.Slots
                .Select(sl =>
                    new GetClassBookScheduleTableForWeekVOSlot(
                        Day: sl.Day,
                        SlotNumber: sl.SlotNumber,
                        Hours: sl.Hours
                            .Select(h => new GetClassBookScheduleTableForWeekVOSlotHour(
                                ScheduleLessonId: h.ScheduleLessonId,
                                CurriculumId: h.CurriculumId,
                                CurriculumGroupName: h.CurriculumGroupName,
                                SubjectName: h.SubjectName,
                                SubjectNameShort: h.SubjectNameShort,
                                SubjectTypeName: h.SubjectTypeName,
                                IsIndividualLesson: h.IsIndividualLesson,
                                IsIndividualCurriculum: h.IsIndividualCurriculum,
                                CurriculumTeachers: h.CurriculumTeachers
                                    .Select(t => new GetClassBookScheduleTableForWeekVOSlotHourTeacher(
                                        t.TeacherPersonId,
                                        t.TeacherFirstName,
                                        t.TeacherLastName,
                                        t.MarkedAsNoReplacement))
                                    .ToArray(),
                                TeacherAbsenceId: h.TeacherAbsenceId,
                                ReplTeacher: h.ReplTeacher == null
                                    ? null
                                    : new GetClassBookScheduleTableForWeekVOSlotHourTeacher(
                                        h.ReplTeacher.TeacherPersonId,
                                        h.ReplTeacher.TeacherFirstName,
                                        h.ReplTeacher.TeacherLastName,
                                        false),
                                ReplTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                                ExtTeacherName: h.ExtTeacherName,
                                IsEmptyHour: h.IsEmptyHour,
                                Location: h.Location))
                            .ToArray()
                    ))
                .ToArray(),
            OffDays: offDays
        );
    }

    public async Task<GetSchoolYearSettings> GetSchoolYearSettingsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from c in this.DbContext.Set<ClassBookSchoolYearSettings>()
            where c.SchoolYear == schoolYear && c.ClassBookId == classBookId
            select new GetSchoolYearSettings(
                c.SchoolYearStartDateLimit,
                c.SchoolYearStartDate,
                c.FirstTermEndDate,
                c.SecondTermStartDate,
                c.SchoolYearEndDate,
                c.SchoolYearEndDateLimit)
        ).SingleAsync(ct);
    }

    [Keyless]
    private record GetAllByClassBookInternalVO(
        [property: Column(TypeName = "SMALLINT")] int SchoolYear,
        int ScheduleId,
        int ClassBookId,
        SchoolTerm? Term,
        bool IsRziApproved,
        int ShiftId,
        string ShiftName,
        string StudentNames,
        string Dates);
    private async Task<GetAllByClassBookInternalVO[]> GetAllByClassBookInternalAsync(
        int schoolYear,
        int classBookId,
        bool isIndividualSchedule,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        string offsetSql = "";
        if (offset.HasValue || limit.HasValue)
        {
            offsetSql = $"OFFSET {offset ?? 0} ROWS\n";
        }

        if (limit.HasValue)
        {
            offsetSql += $"FETCH NEXT {limit} ROWS ONLY\n";
        }

        string sql = $"""
            WITH [RangeItems] AS (
                SELECT
                    sd.[SchoolYear],
                    sd.[ScheduleId],
                    sd.[Date],
                    LAG(sd.[Date]) OVER (PARTITION BY sd.[SchoolYear], sd.[ScheduleId] ORDER BY sd.[Date]) AS PrevDate
                FROM
                    [school_books].[ScheduleDate] sd
                    INNER JOIN [school_books].[Schedule] s ON
                        sd.[SchoolYear] = s.[SchoolYear] AND
                        sd.[ScheduleId] = s.[ScheduleId]
                WHERE
                    s.[SchoolYear] = @schoolYear AND
                    s.[ClassBookId] = @classBookId
            ),
            [RangeStarts] AS (
                SELECT
                    [SchoolYear],
                    [ScheduleId],
                    [Date],
                    [PrevDate]
                FROM
                    [RangeItems]
                WHERE
                    -- a range starts if there is no previous item
                    [PrevDate] IS NULL OR
                    -- or if the previous items is not continuous to the current item (diff = 1)
                    DATEDIFF(day, [PrevDate], [Date]) > 1
            ),
            [ScheduleRanges] AS (
                SELECT
                    r.[SchoolYear],
                    r.[ScheduleId],
                    r.[Date] AS [RangeStart],
                    ISNULL(
                        -- the end of the range is the previous date of next range start
                        LEAD(r.[PrevDate]) OVER (PARTITION BY r.[SchoolYear], r.[ScheduleId] ORDER BY r.[Date]),
                        -- or the max date for the partition
                        (SELECT MAX(sd.[Date])
                        FROM [school_books].[ScheduleDate] sd
                        WHERE sd.[SchoolYear] = r.[SchoolYear]
                            AND sd.[ScheduleId] = r.[ScheduleId]
                        )
                    ) AS [RangeEnd]
                FROM
                    [RangeStarts] r
            ),
            [ScheduleDates] AS (
                SELECT
                    [SchoolYear],
                    [ScheduleId],
                    STRING_AGG(
                        CONCAT(
                            CONVERT(NVARCHAR, [RangeStart], 23),
                            '/',
                            CONVERT(NVARCHAR, [RangeEnd], 23)
                        ),
                        ';'
                    ) WITHIN GROUP (ORDER BY [RangeStart] ASC) AS [Dates],
                    MIN([RangeStart]) AS [StartDate],
                    MAX([RangeEnd]) AS [EndDate]
                FROM
                    [ScheduleRanges]
                GROUP BY
                    [SchoolYear],
                    [ScheduleId]
            )
            SELECT
                s.[SchoolYear],
                s.[ScheduleId],
                s.[ClassBookId],
                s.[Term],
                s.[IsRziApproved],
                s.[ShiftId],
                sh.[Name] AS [ShiftName],
                [school_books].[fn_join_names3](p.[FirstName], p.[MiddleName], p.[LastName]) AS StudentNames,
                sd.[Dates]
            FROM
                [school_books].[Schedule] s
                INNER JOIN [school_books].[Shift] sh ON
                    s.[SchoolYear] = sh.[SchoolYear] AND
                    s.[ShiftId] = sh.[ShiftId]
                INNER JOIN [ScheduleDates] sd ON
                    s.[SchoolYear] = sd.[SchoolYear] AND
                    s.[ScheduleId] = sd.[ScheduleId]
                LEFT JOIN [core].[Person] p ON
                    s.[PersonId] = p.[PersonId]
            WHERE
                s.[SchoolYear] = @schoolYear AND
                s.[ClassBookId] = @classBookId AND
                s.[isIndividualSchedule] = @isIndividualSchedule
            ORDER BY
                sd.[Dates]
            {offsetSql}
            """;
        return await this.DbContext.Set<GetAllByClassBookInternalVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("isIndividualSchedule", isIndividualSchedule))
            .ToArrayAsync(ct);
    }

    public async Task<TableResultVO<GetAllByClassBookVO>> GetAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        bool isIndividualSchedule,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var length = await this.DbContext.Set<Schedule>()
            .CountAsync(
                s => s.SchoolYear == schoolYear &&
                    s.ClassBookId == classBookId &&
                    s.IsIndividualSchedule == isIndividualSchedule,
                ct);

        GetAllByClassBookVO[] result = Array.Empty<GetAllByClassBookVO>();
        if (length > 0)
        {
            result = (await this.GetAllByClassBookInternalAsync(schoolYear, classBookId, isIndividualSchedule, offset, limit, ct))
                .Select(s => new GetAllByClassBookVO(
                    s.SchoolYear,
                    s.ScheduleId,
                    s.ClassBookId,
                    s.Term,
                    s.IsRziApproved,
                    s.ShiftId,
                    s.ShiftName,
                    s.StudentNames,
                    s.Dates
                        .Split(";")
                        .Select(p => p.Split("/"))
                        .Select(r => new GetAllByClassBookVODateRange(
                            DateTime.ParseExact(r[0], "yyyy-MM-dd", null),
                            DateTime.ParseExact(r[1], "yyyy-MM-dd", null)))
                        .Aggregate(
                            new List<GetAllByClassBookVODateRange>(),
                            (list, curr) => {
                                if (list.Count > 0 &&
                                    list[^1].EndDate.DayOfWeek == DayOfWeek.Friday &&
                                    curr.StartDate.DayOfWeek == DayOfWeek.Monday &&
                                    list[^1].EndDate.AddDays(3) == curr.StartDate)
                                {
                                    // merge the current range into the last one
                                    list[^1] = list[^1] with { EndDate = curr.EndDate };
                                }
                                else
                                {
                                    list.Add(curr);
                                }

                                return list;
                            },
                            list => list.ToArray())
                        .ToArray()))
                .ToArray();
        }

        return new TableResultVO<GetAllByClassBookVO>(result, length);
    }

    public async Task<GetVO> GetAsync(int schoolYear, int instId, int classBookId, int scheduleId, CancellationToken ct)
    {
        var schedule = await this.DbContext.Set<Schedule>()
            .Where(s => s.SchoolYear == schoolYear &&
                s.ScheduleId == scheduleId)
            .Select(s =>
                new
                {
                    s.IsIndividualSchedule,
                    s.PersonId,
                    s.Term,
                    s.StartDate,
                    s.EndDate,
                    s.ShiftId,
                    s.IncludesWeekend,
                })
            .SingleAsync(ct);

        var scheduleWeeks = await this.DbContext.Set<ScheduleDate>()
            .Where(sd => sd.SchoolYear == schoolYear &&
                sd.ScheduleId == scheduleId)
            .Select(sd =>
                new
                {
                    sd.Year,
                    sd.WeekNumber
                })
            .Distinct()
            .ToArrayAsync(ct);

        var scheduleHours = (await this.DbContext.Set<ScheduleHour>()
            .Where(sd => sd.SchoolYear == schoolYear &&
                sd.ScheduleId == scheduleId)
            .Select(sh =>
                new
                {
                    sh.Day,
                    sh.HourNumber,
                    sh.CurriculumId,
                    sh.Location
                })
            .ToArrayAsync(ct))
            .ToLookup(sh => sh.Day, sh => (sh.HourNumber, sh.CurriculumId, sh.Location));

        var (readOnlyDates, readOnlyHours) =
            await this.GetScheduleReadOnlyDatesHoursAsync(schoolYear, instId, classBookId, scheduleId, ct);

        var readOnlyWeeksHashSet = readOnlyDates
            .Select(ud =>
                new
                {
                    Year = ud.GetIsoWeekYear(),
                    WeekNumber = ud.GetIsoWeek()
                })
            .Distinct()
            .ToHashSet();

        var readOnlyHoursSet = readOnlyHours
            .ToHashSet();
        var readOnlyHoursPerDayLookup = readOnlyHours.ToLookup(h => h.day);

        return new GetVO(
            schedule.IsIndividualSchedule,
            schedule.PersonId,
            schedule.Term,
            schedule.StartDate,
            schedule.EndDate,
            schedule.IncludesWeekend,
            schedule.ShiftId,
            scheduleWeeks
                .Select(w => new GetVOWeek(
                    w.Year,
                    w.WeekNumber,
                    readOnlyWeeksHashSet.Contains(w)
                ))
                .ToArray(),
            Schedule.ScheduleIsoDays
                .Select(day => new GetVODay(
                    day,
                    scheduleHours[day]
                        .Select(sh => new GetVODayHour(
                            sh.HourNumber,
                            sh.CurriculumId,
                            readOnlyHoursSet.Contains((day, sh.HourNumber, sh.CurriculumId)),
                            sh.Location))
                        .ToArray(),
                    readOnlyHoursPerDayLookup[day].Any()))
                .ToArray(),
            readOnlyDates);
    }

    public async Task<GetUsedDatesWeeksVO> GetUsedDatesWeeksAsync(int schoolYear, int classBookId, bool isIndividualSchedule, int? personId, int? exceptScheduleId, CancellationToken ct)
    {
        var usedDates = await this.GetUsedDatesAsync(schoolYear, classBookId, isIndividualSchedule, personId, exceptScheduleId, ct);

        var usedWeeks =
            usedDates.GroupBy(d => new { Year = d.GetIsoWeekYear(), WeekNumber = d.GetIsoWeek() })
            .Select(g =>
                new GetUsedDatesWeeksVOWeek(
                    g.Key.Year,
                    g.Key.WeekNumber,
                    Schedule.ScheduleIsoDays.Except(g.Select(d => d.GetIsoDayOfWeek())).Any()))
            .OrderBy(uw => uw.Year)
            .ThenBy(uw => uw.WeekNumber)
            .ToArray();

        return new GetUsedDatesWeeksVO(usedDates, usedWeeks);
    }

    public async Task<DateTime[]> GetUsedDatesAsync(int schoolYear, int classBookId, bool isIndividualSchedule, int? personId, int? exceptScheduleId, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Schedule>();

        predicate = predicate.And(s =>
            s.SchoolYear == schoolYear &&
            s.ClassBookId == classBookId);

        if (exceptScheduleId != null)
        {
            predicate = predicate.And(s => s.ScheduleId != exceptScheduleId);
        }

        if (!isIndividualSchedule)
        {
            predicate = predicate.And(s => !s.IsIndividualSchedule);
        }
        else
        {
            predicate = predicate.And(s => s.IsIndividualSchedule && s.PersonId == personId);
        }

        return await (
            from s in this.DbContext.Set<Schedule>().Where(predicate)
            join d in this.DbContext.Set<ScheduleDate>()
                on new { s.SchoolYear, s.ScheduleId }
                equals new { d.SchoolYear, d.ScheduleId }
            orderby d.Date
            select d.Date
        )
        .ToArrayAsync(ct);
    }

    public async Task<DateTime[]> GetOffDatesAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from od in this.DbContext.Set<ClassBookOffDayDate>()

            where od.SchoolYear == schoolYear &&
                od.ClassBookId == classBookId

            select od.Date
        ).ToArrayAsync(ct);
    }

    public async Task<GetOffDatesPgVO[]> GetOffDatesPgAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from od in this.DbContext.Set<ClassBookOffDayDate>()

            where od.SchoolYear == schoolYear &&
                od.ClassBookId == classBookId

            select new GetOffDatesPgVO(
                od.Date,
                od.IsPgOffProgramDay
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetShiftHoursForValidationVO[]> GetShiftHoursForValidationAsync(
        int schoolYear,
        int instId,
        int shiftId,
        CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Shift>()

            join sh in this.DbContext.Set<ShiftHour>()
            on new { s.SchoolYear, s.ShiftId } equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                s.InstId == instId &&
                s.ShiftId == shiftId

            select new GetShiftHoursForValidationVO(
                    sh.Day,
                    sh.HourNumber)
        ).ToArrayAsync(ct);
    }

    public async Task<GetScheduleUsedHoursVO[]> GetScheduleUsedHoursAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        CancellationToken ct)
    {
        return await this.GetUsedLessonsQuery(schoolYear, instId, classBookId, scheduleId)
            .Select(l => new { l.Day, l.HourNumber })
            .Distinct()
            .Select(l => new GetScheduleUsedHoursVO(l.Day, l.HourNumber))
            .ToArrayAsync(ct);
    }

    public async Task<GetScheduleUsedHoursTableVO[]> GetScheduleUsedHoursTableAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        int day,
        CancellationToken ct)
    {
        var res = await (
            from ul in this.GetUsedLessonsQuery(schoolYear, instId, classBookId, scheduleId)
            join c in this.DbContext.Set<Curriculum>() on ul.CurriculumId equals c.CurriculumId
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join tap in this.DbContext.Set<Person>()
            on ul.TeacherAbsenceTeacherPersonId equals tap.PersonId
            into j1 from tap in j1.DefaultIfEmpty()

            join lsp in this.DbContext.Set<Person>()
            on ul.LectureScheduleTeacherPersonId equals lsp.PersonId
            into j2
            from lsp in j2.DefaultIfEmpty()

            where ul.Day == day

            select new
            {
                ul.HourNumber,
                ul.AbsenceId,
                ul.GradeId,
                ul.TopicId,
                Subject = s.SubjectName,
                SubjectType = st.Name,
                ul.Date,
                ul.TeacherAbsenceStartDate,
                ul.TeacherAbsenceEndDate,
                TeacherAbsenceTeacherPersonName = StringUtils.JoinNames(tap.FirstName, tap.LastName),
                ul.LectureScheduleStartDate,
                ul.LectureScheduleEndDate,
                LectureScheduleTeacherPersonName = StringUtils.JoinNames(lsp.FirstName, lsp.LastName)
            }
        ).ToArrayAsync(ct);

        return res.GroupBy(
            r => new { r.HourNumber, r.Subject, r.SubjectType },
            (k, g) => new GetScheduleUsedHoursTableVO(
                k.HourNumber,
                $"{k.Subject} / {k.SubjectType}",
                g.Where(r => r.AbsenceId != null).Select(r => r.Date).Distinct().OrderBy(d => d).ToArray(),
                g.Where(r => r.GradeId != null).Select(r => r.Date).Distinct().OrderBy(d => d).ToArray(),
                g.Where(r => r.TopicId != null).Select(r => r.Date).Distinct().OrderBy(d => d).ToArray(),
                g.Where(r => r.TeacherAbsenceStartDate != null)
                    .Select(r => new GetScheduleUsedHoursTableVOTeacherAbsence(
                        r.TeacherAbsenceStartDate!.Value,
                        r.TeacherAbsenceEndDate!.Value,
                        r.TeacherAbsenceTeacherPersonName))
                    .OrderBy(r => r.StartDate)
                    .ToArray(),
                g.Where(r => r.LectureScheduleStartDate != null)
                    .Select(r => new GetScheduleUsedHoursTableVOLectureSchedule(
                        r.LectureScheduleStartDate!.Value,
                        r.LectureScheduleEndDate!.Value,
                        r.LectureScheduleTeacherPersonName))
                    .OrderBy(r => r.StartDate)
                    .ToArray()
            )
        ).OrderBy(r => r.HourNumber).ToArray();
    }

    public async Task<GetTeacherScheduleTableForWeekVO> GetTeacherScheduleTableForWeekAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        int year,
        int weekNumber,
        CancellationToken ct)
    {
        DateTime startDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1);
        DateTime endDate = DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7);

        Expression<Func<SchedulesQueryHelper.GetScheduleHoursVO, bool>> scheduleHourPredicate = h =>
                string.IsNullOrEmpty(h.ExtTeacherName) && (h.CurriculumTeachers.Any(t => t.TeacherPersonId == teacherPersonId) || (h.IsEmptyHour == false && h.ReplTeacher!.TeacherPersonId == teacherPersonId)) &&
                h.Date >= startDate.Date &&
                h.Date <= endDate.Date;

        var scheduleTable =
            await this.DbContext.GetScheduleTableAsync(
                includeUsedHoursInfo: true,
                schoolYear,
                cb => cb.InstId == instId && cb.IsValid,
                s => true,
                scheduleHourPredicate,
                ct: ct);

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

        return new GetTeacherScheduleTableForWeekVO(
            ShiftHours:
                scheduleTable.ShiftHours
                .Select(h =>
                    new GetTeacherScheduleTableForWeekVOShiftHour(
                        SlotNumber: h.SlotNumber,
                        StartTime: h.StartTime,
                        EndTime: h.EndTime))
                .ToArray(),
            Slots:
                scheduleTable.Slots
                .Select(sl =>
                    new GetTeacherScheduleTableForWeekVOSlot(
                        Day: sl.Day,
                        SlotNumber: sl.SlotNumber,
                        Hours: sl.Hours
                            .Where(h =>
                                !offDays.Contains((h.ClassBookId, h.Date)) &&
                                h.CurriculumTeachers.Any(t => t.ActiveAtLessonTime) &&
                                (h.ReplTeacher == null || h.ReplTeacher.TeacherPersonId == teacherPersonId))
                            .Select(h => new GetTeacherScheduleTableForWeekVOSlotHour(
                                ScheduleLessonId: h.ScheduleLessonId,
                                CurriculumId: h.CurriculumId,
                                BookType: h.BookType,
                                BasicClassId: h.BasicClassId,
                                ClassBookId: h.ClassBookId,
                                ClassBookFullName: h.ClassBookFullName,
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
                                IsEmptyHour: h.IsEmptyHour,
                                IsReplTeacher: h.ReplTeacher != null,
                                ReplTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                                Location: h.Location))
                            .ToArray()
                    ))
                .ToArray()
        );
    }

    public async Task<GetScheduleCurriculumInfoVO[]> GetScheduleCurriculumsInfoAsync(int schoolYear, int instId, int[] curriculumIds, CancellationToken ct)
    {
        return await (
                from c in this.DbContext.Set<Curriculum>()

                join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
                join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

                where c.InstitutionId == instId &&
                      c.SchoolYear == schoolYear &&
                      this.DbContext.MakeIdsQuery(curriculumIds).Any(id => c.CurriculumId == id.Id)

                select new
                {
                    c.CurriculumId,
                    CurriculumName = $"{s.SubjectName} / {st.Name}",
                    c.IsIndividualCurriculum
                }
            )
            .Select(c =>
                new GetScheduleCurriculumInfoVO(
                    c.CurriculumId,
                    c.CurriculumName,
                    c.IsIndividualCurriculum ?? false))
            .ToArrayAsync(ct);
    }

    private record GetUsedLessonsVO
    {
        public DateTime Date { get; init; }
        public int Day { get; init; }
        public int HourNumber { get; init; }
        public int CurriculumId { get; init; }
        public int? AbsenceId { get; init; }
        public int? GradeId { get; init; }
        public int? TopicId { get; init; }
        public DateTime? TeacherAbsenceStartDate { get; init; }
        public DateTime? TeacherAbsenceEndDate { get; init; }
        public int? TeacherAbsenceTeacherPersonId { get; init; }
        public DateTime? LectureScheduleStartDate { get; init; }
        public DateTime? LectureScheduleEndDate { get; init; }
        public int? LectureScheduleTeacherPersonId { get; init; }
    }
    private IQueryable<GetUsedLessonsVO> GetUsedLessonsQuery(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId)
    {
        var usedScheduleLessons =
            (from a in this.DbContext.Set<Absence>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId

            select new
            {
                a.SchoolYear,
                a.ScheduleLessonId,
                AbsenceId = (int?)a.AbsenceId,
                GradeId = (int?)null,
                TopicId = (int?)null,
                TeacherAbsenceStartDate = (DateTime?)null,
                TeacherAbsenceEndDate = (DateTime?)null,
                TeacherAbsenceTeacherPersonId = (int?)null,
                LectureScheduleStartDate = (DateTime?)null,
                LectureScheduleEndDate = (DateTime?)null,
                LectureScheduleTeacherPersonId = (int?)null
            })
            .Union(
                from g in this.DbContext.Set<Grade>()

                where g.SchoolYear == schoolYear &&
                    g.ClassBookId == classBookId &&
                    g.ScheduleLessonId != null

                select new
                {
                    g.SchoolYear,
                    ScheduleLessonId = g.ScheduleLessonId!.Value,
                    AbsenceId = (int?)null,
                    GradeId = (int?)g.GradeId,
                    TopicId = (int?)null,
                    TeacherAbsenceStartDate = (DateTime?)null,
                    TeacherAbsenceEndDate = (DateTime?)null,
                    TeacherAbsenceTeacherPersonId = (int?)null,
                    LectureScheduleStartDate = (DateTime?)null,
                    LectureScheduleEndDate = (DateTime?)null,
                    LectureScheduleTeacherPersonId = (int?)null
                }
            )
            .Union(
                from t in this.DbContext.Set<Topic>()

                where t.SchoolYear == schoolYear &&
                    t.ClassBookId == classBookId

                select new
                {
                    t.SchoolYear,
                    t.ScheduleLessonId,
                    AbsenceId = (int?)null,
                    GradeId = (int?)null,
                    TopicId = (int?)t.TopicId,
                    TeacherAbsenceStartDate = (DateTime?)null,
                    TeacherAbsenceEndDate = (DateTime?)null,
                    TeacherAbsenceTeacherPersonId = (int?)null,
                    LectureScheduleStartDate = (DateTime?)null,
                    LectureScheduleEndDate = (DateTime?)null,
                    LectureScheduleTeacherPersonId = (int?)null
                }
            )
            .Union(
                from tah in this.DbContext.Set<TeacherAbsenceHour>()

                join ta in this.DbContext.Set<TeacherAbsence>()
                on new { tah.SchoolYear, tah.TeacherAbsenceId }
                equals new { ta.SchoolYear, ta.TeacherAbsenceId }

                where ta.SchoolYear == schoolYear &&
                    ta.InstId == instId

                select new
                {
                    tah.SchoolYear,
                    tah.ScheduleLessonId,
                    AbsenceId = (int?)null,
                    GradeId = (int?)null,
                    TopicId = (int?)null,
                    TeacherAbsenceStartDate = (DateTime?)ta.StartDate,
                    TeacherAbsenceEndDate = (DateTime?)ta.EndDate,
                    TeacherAbsenceTeacherPersonId = (int?)ta.TeacherPersonId,
                    LectureScheduleStartDate = (DateTime?)null,
                    LectureScheduleEndDate = (DateTime?)null,
                    LectureScheduleTeacherPersonId = (int?)null
                }
            )
            .Union(
                from lsh in this.DbContext.Set<LectureScheduleHour>()

                join ls in this.DbContext.Set<LectureSchedule>()
                on new { lsh.SchoolYear, lsh.LectureScheduleId }
                equals new { ls.SchoolYear, ls.LectureScheduleId }

                where ls.SchoolYear == schoolYear &&
                    ls.InstId == instId

                select new
                {
                    lsh.SchoolYear,
                    lsh.ScheduleLessonId,
                    AbsenceId = (int?)null,
                    GradeId = (int?)null,
                    TopicId = (int?)null,
                    TeacherAbsenceStartDate = (DateTime?)null,
                    TeacherAbsenceEndDate = (DateTime?)null,
                    TeacherAbsenceTeacherPersonId = (int?)null,
                    LectureScheduleStartDate = (DateTime?)ls.StartDate,
                    LectureScheduleEndDate = (DateTime?)ls.EndDate,
                    LectureScheduleTeacherPersonId = (int?)ls.TeacherPersonId
                }
            );

        return from usl in usedScheduleLessons

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { usl.SchoolYear, usl.ScheduleLessonId }
            equals new { sl.SchoolYear, sl.ScheduleLessonId }

            where sl.SchoolYear == schoolYear &&
                sl.ScheduleId == scheduleId

            // using the property initializer, as EF throws that
            // it cannot translate the query if a constructor is used
            select new GetUsedLessonsVO
            {
                Date = sl.Date,
                Day = sl.Day,
                HourNumber = sl.HourNumber,
                CurriculumId = sl.CurriculumId,
                AbsenceId = usl.AbsenceId,
                GradeId = usl.GradeId,
                TopicId = usl.TopicId,
                TeacherAbsenceStartDate = usl.TeacherAbsenceStartDate,
                TeacherAbsenceEndDate = usl.TeacherAbsenceEndDate,
                TeacherAbsenceTeacherPersonId = usl.TeacherAbsenceTeacherPersonId,
                LectureScheduleStartDate = usl.LectureScheduleStartDate,
                LectureScheduleEndDate = usl.LectureScheduleEndDate,
                LectureScheduleTeacherPersonId = usl.LectureScheduleTeacherPersonId,
            };
    }

    private async Task<(DateTime[] dates, (int day, int hourNumber, int curriculumId)[] hours)> GetScheduleReadOnlyDatesHoursAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        CancellationToken ct)
    {
        var usedLessons = await this.GetUsedLessonsQuery(schoolYear, instId, classBookId, scheduleId).ToArrayAsync(ct);

        return (
            dates: usedLessons
                .Select(sl => sl.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToArray(),
            hours: usedLessons.Select(sl => new { sl.Day, sl.HourNumber, sl.CurriculumId })
                .Distinct()
                .Select(sl => (
                    day: sl.Day,
                    hourNumber: sl.HourNumber,
                    curriculumId: sl.CurriculumId))
                .ToArray()
        );
    }
}
