namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IScheduleAndAbsencesByTermReportsQueryRepository;

internal class ScheduleAndAbsencesByTermReportsQueryRepository : Repository, IScheduleAndAbsencesByTermReportsQueryRepository
{
    private readonly int SqlCommandTimeout = 180;

    public ScheduleAndAbsencesByTermReportsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<ScheduleAndAbsencesByTermReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.ScheduleAndAbsencesByTermReportId,
                r.SchoolYear,
                EnumUtils.GetEnumDescription(r.Term),
                r.ClassBookName,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<ScheduleAndAbsencesByTermReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId

            select new GetVO(
                r.SchoolYear,
                r.ScheduleAndAbsencesByTermReportId,
                r.Term,
                r.ClassBookName,
                r.IsDPLR,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<GetWeeksVO[]> GetWeeksAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct)
    {
        return await (
            from rw in this.DbContext.Set<ScheduleAndAbsencesByTermReportWeek>()
            join r in this.DbContext.Set<ScheduleAndAbsencesByTermReport>() on new { rw.SchoolYear, rw.ScheduleAndAbsencesByTermReportId } equals new { r.SchoolYear, r.ScheduleAndAbsencesByTermReportId }
            where rw.SchoolYear == schoolYear && r.InstId == instId && rw.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId
            orderby rw.ScheduleAndAbsencesByTermReportWeekId

            select new GetWeeksVO(
                rw.StudentName,
                rw.WeekName,
                rw.AdditionalActivities,
                rw.Days.OrderBy(w => w.Date).Select(d => new GetWeeksVODay(
                    d.Date,
                    d.DayName,
                    d.IsOffDay,
                    d.IsEmptyDay,
                    d.Hours.OrderBy(w => w.HourNumber).Select(h => new GetWeekVODayHour(
                        h.HourNumber,
                        h.IsEmptyHour,
                        h.CurriculumName,
                        h.CurriculumTeacherNames,
                        h.ExcusedStudentClassNumbers,
                        h.UnexcusedStudentClassNumbers,
                        h.LateStudentClassNumbers,
                        h.DplrAbsenceStudentClassNumbers,
                        h.DplrAttendanceStudentClassNumbers,
                        h.Topics))
                    .ToArray()))
                .ToArray())
        ).ToArrayAsync(ct);
    }

    public async Task<GetWeeksForAddVO[]> GetWeeksForAddAsync(
        int schoolYear,
        int instId,
        SchoolTerm term,
        int classBookId,
        CancellationToken ct)
    {
        // Increase SQL command timeout to avoid timeout exceptions
        this.DbContext.Database.SetCommandTimeout(this.SqlCommandTimeout);

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

        var predicate = PredicateBuilder.True<ScheduleLesson>();

        predicate = term switch
        {
            SchoolTerm.TermOne => predicate.And(q => q.Date >= schoolYearSettings.SchoolYearStartDate && q.Date <= schoolYearSettings.FirstTermEndDate),
            SchoolTerm.TermTwo => predicate.And(q => q.Date >= schoolYearSettings.SecondTermStartDate && q.Date <= schoolYearSettings.SchoolYearEndDate),
            _ => throw new Exception("Invalid term"),
        };

        var scheduleHours = await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>().Where(predicate)
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join sd in this.DbContext.Set<ScheduleDate>()
            on new { sl.SchoolYear, sl.ScheduleId, sl.Date } equals new { sd.SchoolYear, sd.ScheduleId, sd.Date }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join stp in this.DbContext.Set<Person>()
            on new { sch.PersonId } equals new { PersonId = (int?)stp.PersonId }
            into g3
            from stp in g3.DefaultIfEmpty()

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId

            select new
            {
                sl.ScheduleLessonId,
                sl.Date,
                sl.Day,
                sl.HourNumber,
                sd.Year,
                sd.WeekNumber,

                StudentPersonId = (int?)stp.PersonId,
                StudentName = StringUtils.JoinNames(stp.FirstName, stp.MiddleName, stp.LastName),

                sch.ShiftId,

                CurriculumName = $"{s.SubjectName} / {st.Name}",

                TeacherAbsence = (
                    from tah in this.DbContext.Set<TeacherAbsenceHour>()

                    join repl in this.DbContext.Set<Person>()
                    on new { PersonId = tah.ReplTeacherPersonId } equals new { PersonId = (int?)repl.PersonId }
                    into g1 from repl in g1.DefaultIfEmpty()

                    where tah.SchoolYear == sl.SchoolYear &&
                        tah.ScheduleLessonId == sl.ScheduleLessonId

                    select new
                    {
                        tah.TeacherAbsenceId,
                        tah.ReplTeacherIsNonSpecialist,
                        ReplTeacherName = StringUtils.JoinNames(repl.FirstName, repl.LastName),
                        tah.ExtReplTeacherName,
                        IsEmptyHour = tah.ReplTeacherPersonId == null && string.IsNullOrEmpty(tah.ExtReplTeacherName)
                    }
                ).SingleOrDefault(),

                CurriculumTeachers = (
                    from ctr in this.DbContext.Set<CurriculumTeacher>()

                    join sp in this.DbContext.Set<StaffPosition>()
                    on ctr.StaffPositionId equals sp.StaffPositionId

                    join p in this.DbContext.Set<Person>()
                    on sp.PersonId equals p.PersonId

                    where ctr.CurriculumId == sl.CurriculumId &&
                          ((ctr.SchoolYear <= 2022 && ctr.IsValid) ||   // Getting only valid teachers for the date of the lesson
                           (ctr.SchoolYear > 2022 &&
                            (ctr.StaffPositionStartDate ?? ctr.ValidFrom ?? new DateTime(schoolYear, 9, 1)) <= sl.Date &&
                            (ctr.StaffPositionTerminationDate == null || ctr.StaffPositionTerminationDate >= sl.Date)))

                    select StringUtils.JoinNames(p.FirstName, p.LastName)
                ).ToArray(),

                Topics = (
                    from tt in this.DbContext.Set<TopicTitle>()
                    join t in this.DbContext.Set<Topic>()
                    on new { tt.SchoolYear, tt.TopicId } equals new { t.SchoolYear, t.TopicId }
                    where t.SchoolYear == sch.SchoolYear &&
                        t.ClassBookId == sch.ClassBookId &&
                        t.ScheduleLessonId == sl.ScheduleLessonId
                    select tt.Title
                ).ToArray()
            }
        )
        .AsSplitQuery()
        .ToArrayAsync(ct);

        var offDayDates = await (
            from od in this.DbContext.Set<ClassBookOffDayDate>()

            where
                od.SchoolYear == schoolYear &&
                od.ClassBookId == classBookId

            select
                od.Date
        ).ToArrayAsync(ct);

        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId &&
                cb.IsValid
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var absences = (await (
                from a in this.DbContext.Set<Absence>()

                join p in this.DbContext.Set<Person>() on a.PersonId equals p.PersonId

                join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
                on a.PersonId equals sc.PersonId
                into g1
                from sc in g1.DefaultIfEmpty()

                where a.SchoolYear == schoolYear &&
                     a.ClassBookId == classBookId

                select new
                {
                    a.ScheduleLessonId,
                    a.Type,
                    sc.ClassNumber,
                    StudentName = (string?)StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                }
            )
            .ToArrayAsync(ct))
            .ToLookup(a => a.ScheduleLessonId);

        var additionalActivities = (await (
                from t in this.DbContext.Set<AdditionalActivity>()

                where t.SchoolYear == schoolYear &&
                    t.ClassBookId == classBookId

                select new
                {
                    t.Year,
                    t.WeekNumber,
                    t.Activity
                }
            )
            .ToArrayAsync(ct))
            .ToLookup(a => new { a.Year, a.WeekNumber }, a => a.Activity);

        var shiftIds = scheduleHours.Select(h => h.ShiftId).Distinct().ToArray();

        var shiftHours = await (
            from s in this.DbContext.Set<Shift>()

            join sh in this.DbContext.Set<ShiftHour>()
            on new { s.SchoolYear, s.ShiftId } equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(shiftIds)
                    .Any(id => s.ShiftId == id.Id)

            select new
            {
                s.ShiftId,
                sh.Day,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime
            }
        ).ToArrayAsync(ct);

        var scheduleIncludesWeekend = scheduleHours.Any(sh => sh.Day > 5);

        return scheduleHours
            .GroupBy(r => new { r.StudentPersonId, r.StudentName, r.Year, r.WeekNumber })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.WeekNumber)
            .ThenBy(g => g.Key.StudentName)
            .Select(g => new GetWeeksForAddVO
            (
                StudentName: g.Key.StudentName,
                Year: g.Key.Year,
                WeekNumber: g.Key.WeekNumber,
                WeekName: $"{DateExtensions.GetDateFromIsoWeek(g.Key.Year, g.Key.WeekNumber, 1):dd.MM} " +
                    $"- {DateExtensions.GetDateFromIsoWeek(g.Key.Year, g.Key.WeekNumber, 7):dd.MM}",
                AdditionalActivities: string.Join(", ", additionalActivities[new { g.Key.Year, g.Key.WeekNumber }]),
                Days: Enumerable.Range(1, scheduleIncludesWeekend ? 7 : 5).Select(day =>
                {
                    var date = DateExtensions.GetDateFromIsoWeek(g.Key.Year, g.Key.WeekNumber, day);
                    var shiftId = g.FirstOrDefault(h => h.Day == day)?.ShiftId;

                    if (shiftId == null)
                    {
                        return new GetWeeksForAddVODay(
                            date,
                            date.ToString("dddd"),
                            offDayDates.Contains(date),
                            true,
                            Array.Empty<GetWeeksForAddVODayHour>()
                        );
                    }

                    var shours = shiftHours.Where(sh => sh.ShiftId == shiftId.Value && sh.Day == day);

                    var hours = Enumerable.Range(shours.DefaultIfEmpty().Min(s => s?.HourNumber ?? 0), shours.Count()).SelectMany((hourNumber, index) =>
                    {
                        var hours = g.Where(h => h.Day == day && h.HourNumber == hourNumber).ToArray();
                        if (!hours.Any())
                        {
                            return new GetWeeksForAddVODayHour[]
                            {
                                new GetWeeksForAddVODayHour(
                                    hourNumber,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    null
                                )
                            };
                        }
                        else
                        {
                            return hours.Select(h =>
                                new GetWeeksForAddVODayHour(
                                    hourNumber,
                                    h.TeacherAbsence?.ReplTeacherIsNonSpecialist == true ? "Гражданско образование" : h.CurriculumName,
                                    string.IsNullOrEmpty(h.TeacherAbsence?.ReplTeacherName) && string.IsNullOrEmpty(h.TeacherAbsence?.ExtReplTeacherName) ? string.Join(", ", h.CurriculumTeachers) :
                                        !string.IsNullOrEmpty(h.TeacherAbsence?.ReplTeacherName) ? h.TeacherAbsence?.ReplTeacherName + " (зам.)" : h.TeacherAbsence?.ExtReplTeacherName + " (външен лектор)",
                                    h.TeacherAbsence?.IsEmptyHour,
                                    string.Join(", ", absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.Excused).Select(a => a.ClassNumber?.ToString() ?? a.StudentName)),
                                    string.Join(", ", absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.Unexcused).Select(a => a.ClassNumber?.ToString() ?? a.StudentName)),
                                    string.Join(", ", absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.Late).Select(a => a.ClassNumber?.ToString() ?? a.StudentName)),
                                    string.Join(", ", absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.DplrAbsence).Select(a => a.ClassNumber?.ToString() ?? a.StudentName)),
                                    string.Join(", ", absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.DplrAttendance).Select(a => a.ClassNumber?.ToString() ?? a.StudentName)),
                                    string.Join(", ", h.Topics)));
                        }
                    });

                    return new GetWeeksForAddVODay(
                        date,
                        date.ToString("dddd"),
                        offDayDates.Contains(date),
                        false,
                        hours.ToArray()
                    );
                })
                .ToArray()
            ))
            .ToArray();
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<ScheduleAndAbsencesByTermReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId
            select mtr.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct)
    {
        var weeks = await (
            from rw in this.DbContext.Set<ScheduleAndAbsencesByTermReportWeek>()
            join r in this.DbContext.Set<ScheduleAndAbsencesByTermReport>() on new { rw.SchoolYear, rw.ScheduleAndAbsencesByTermReportId } equals new { r.SchoolYear, r.ScheduleAndAbsencesByTermReportId }
            where rw.SchoolYear == schoolYear && r.InstId == instId && rw.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId
            orderby rw.ScheduleAndAbsencesByTermReportWeekId

            select new GetExcelDataVOWeek(
                rw.StudentName,
                rw.WeekName,
                rw.AdditionalActivities,
                rw.Days.OrderBy(w => w.Date).Select(d => new GetExcelDataVOWeekDay(
                    d.Date,
                    d.DayName,
                    d.IsOffDay,
                    d.IsEmptyDay,
                    d.Hours.OrderBy(d => d.HourNumber).Select(h => new GetExcelDataVOWeekDayHour(
                        h.HourNumber,
                        h.IsEmptyHour,
                        h.CurriculumName,
                        h.CurriculumTeacherNames,
                        h.ExcusedStudentClassNumbers,
                        h.UnexcusedStudentClassNumbers,
                        h.LateStudentClassNumbers,
                        h.DplrAbsenceStudentClassNumbers,
                        h.DplrAttendanceStudentClassNumbers,
                        h.Topics))
                    .ToArray()))
                .ToArray())
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<ScheduleAndAbsencesByTermReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId

            select new GetExcelDataVO
            (
                EnumUtils.GetEnumDescription(r.Term),
                r.ClassBookName,
                r.IsDPLR,
                r.CreateDate,
                weeks
            )
        ).SingleAsync(ct);
    }
}
