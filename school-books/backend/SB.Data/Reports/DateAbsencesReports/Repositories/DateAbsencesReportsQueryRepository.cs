namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IDateAbsencesReportsQueryRepository;

internal class DateAbsencesReportsQueryRepository : Repository, IDateAbsencesReportsQueryRepository
{
    public DateAbsencesReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<DateAbsencesReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SchoolYear,
                r.DateAbsencesReportId,
                r.ReportDate,
                r.IsUnited,
                r.ClassBookNames,
                r.ShiftNames,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<DateAbsencesReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.DateAbsencesReportId == dateAbsencesReportId

            select new GetVO(
                r.SchoolYear,
                r.DateAbsencesReportId,
                r.ReportDate,
                r.IsUnited,
                r.ClassBookNames,
                r.ShiftNames,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<GetItemsVO[]> GetItemsAsync(
        int schoolYear,
        int dateAbsencesReportId,
        CancellationToken ct)
    {
        return (await (
            from ri in this.DbContext.Set<DateAbsencesReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.DateAbsencesReportId == dateAbsencesReportId

            orderby ri.DateAbsencesReportItemId

            select new
            {
                ri.DateAbsencesReportItemId,
                ri.ClassBookId,
                ri.ClassBookName,
                ri.ShiftId,
                ri.ShiftName,
                ri.HourNumber,
                ri.AbsenceStudentNumbers,
                ri.AbsenceStudentCount,
                ri.IsOffDay,
                ri.HasScheduleDate
            }
        ).ToArrayAsync(ct))
        .GroupBy(g => new
        {
            g.ClassBookId,
            g.ClassBookName,
            g.IsOffDay,
            g.HasScheduleDate
        })
        .Select(g => new GetItemsVO(
            g.Key.ClassBookId,
            g.Key.ClassBookName,
            g.Key.IsOffDay,
            g.Key.HasScheduleDate,
            g.OrderBy(r => r.DateAbsencesReportItemId)
            .Select(c => new GetItemsVOHour(
                c.ShiftId,
                c.ShiftName,
                c.HourNumber,
                c.AbsenceStudentNumbers,
                c.AbsenceStudentCount))
            .ToArray()
        ))
        .ToArray();
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        DateTime reportDate,
        bool isUnited,
        int[] classBookIds,
        int[] shiftIds,
        CancellationToken ct)
    {
        // make sure we use only date part
        var date = reportDate.Date;

        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        var shiftPredicate = PredicateBuilder.True<Shift>();

        classBookPredicate = classBookPredicate.And(
            cb => cb.SchoolYear == schoolYear &&
            cb.InstId == instId &&
            cb.IsValid);

        shiftPredicate = shiftPredicate.And(s => s.SchoolYear == schoolYear && s.InstId == instId);

        if (classBookIds.Any())
        {
            classBookPredicate = classBookPredicate.And(g => this.DbContext.MakeIdsQuery(classBookIds).Any(cid => g.ClassBookId == cid.Id));
        }

        if (shiftIds.Any())
        {
            shiftPredicate = shiftPredicate.And(g => this.DbContext.MakeIdsQuery(shiftIds).Any(cid => g.ShiftId == cid.Id));
        }

        var offDays = (await (
            from odd in this.DbContext.Set<ClassBookOffDayDate>()
            join od in this.DbContext.Set<OffDay>() on new { odd.SchoolYear, odd.OffDayId } equals new { od.SchoolYear, od.OffDayId }
            where
                od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                odd.Date == date

            select odd.ClassBookId
        )
        .ToArrayAsync(ct))
        .ToHashSet();

        var classBooksWithSchedule = (await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)

            join sch in this.DbContext.Set<Schedule>()
            on new { cb.SchoolYear, cb.ClassBookId } equals new { sch.SchoolYear, sch.ClassBookId }

            join sl in this.DbContext.Set<ScheduleDate>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            where sl.Date == date

            select cb.ClassBookId
        )
        .ToArrayAsync(ct))
        .ToHashSet();

        var shifts = await (
            from s in this.DbContext.Set<Shift>().Where(shiftPredicate)
            orderby s.Hours.Max(h => h.StartTime)

            select new
            {
                ShiftId = (int?)s.ShiftId,
                ShiftName = (string?)s.Name,
                ShiftHours = s.Hours.Where(h => h.Day == (int)date.DayOfWeek + 1).OrderBy(sh => sh.StartTime).Select(sh =>  sh.HourNumber).ToArray()
            }
        ).ToArrayAsync(ct);

        var nonCombinedClassBookStudents =
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals new { cg.SchoolYear, ClassId = cg.ParentClassId }
            join sc in this.DbContext.Set<StudentClass>() on new { cg.SchoolYear, cg.InstitutionId, cg.ClassId } equals new { sc.SchoolYear, sc.InstitutionId, sc.ClassId }
            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            select new
            {
                cb.BookType,
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                p.PersonId,
                sc.ClassNumber
            };

        var combinedClassBookStudents =
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)
            join sc in this.DbContext.Set<StudentClass>() on new { cb.SchoolYear, cb.ClassId } equals new { sc.SchoolYear, sc.ClassId }
            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            select new
            {
                cb.BookType,
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                p.PersonId,
                sc.ClassNumber
            };

        var allClassBooksStudents = nonCombinedClassBookStudents.Union(combinedClassBookStudents).OrderBy(cb => cb.BasicClassId).ThenBy(c => c.ClassBookName);

        var absences = await (
            from a in this.DbContext.Set<Absence>()
            join cb in this.DbContext.Set<ClassBook>() on new { a.SchoolYear, a.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join sl in this.DbContext.Set<ScheduleLesson>() on new { a.SchoolYear, a.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }
            join s in this.DbContext.Set<Schedule>() on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }
            join sc in allClassBooksStudents on new { a.ClassBookId, a.PersonId } equals new { sc.ClassBookId, sc.PersonId }

            where cb.InstId == instId && cb.SchoolYear == schoolYear && cb.IsValid && a.Date == date && (a.Type == AbsenceType.Excused || a.Type == AbsenceType.Unexcused || a.Type == AbsenceType.DplrAbsence)

            orderby sc.ClassNumber

            select new
            {
                cb.ClassBookId,
                s.ShiftId,
                a.PersonId,
                sc.ClassNumber,
                sl.HourNumber
            }).ToArrayAsync(ct);

        var attendances = await (
            from a in this.DbContext.Set<Attendance>()
            join cb in this.DbContext.Set<ClassBook>() on new { a.SchoolYear, a.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join s in this.DbContext.Set<Schedule>() on new { cb.SchoolYear, cb.ClassBookId } equals new { s.SchoolYear, s.ClassBookId }
            join sc in allClassBooksStudents on new { a.ClassBookId, a.PersonId } equals new { sc.ClassBookId, sc.PersonId }

            where cb.InstId == instId &&
            cb.SchoolYear == schoolYear &&
            cb.IsValid &&
            a.Date == date &&
            (a.Type == AttendanceType.ExcusedAbsence || a.Type == AttendanceType.UnexcusedAbsence) &&
            this.DbContext.Set<ScheduleDate>()
                .Where(g =>
                    g.SchoolYear == schoolYear &&
                    g.ScheduleId == s.ScheduleId &&
                    g.Date == a.Date)
                .Any()

            orderby sc.ClassNumber

            select new
            {
                cb.ClassBookId,
                s.ShiftId,
                a.PersonId,
                sc.ClassNumber
            }).ToArrayAsync(ct);

        var groupedAttendances = attendances
        .GroupBy(g => new
        {
            g.ClassBookId,
            ShiftId = !isUnited ? (int?)g.ShiftId : null
        })
        .ToDictionary(
            g => g.Key,
            g => new
            {
                AbsenceStudentNumbers = string.Join(", ", g.Select(a => a.ClassNumber)),
                AbsenceStudentCount = g.Count()
            });

        var groupedAbsences = absences
        .GroupBy(g => new
        {
            g.ClassBookId,
            ShiftId = !isUnited ? (int?)g.ShiftId : null,
            g.HourNumber
        })
        .ToDictionary(
            g => g.Key,
            g => new
            {
                AbsenceStudentNumbers = string.Join(", ", g.Select(a => a.ClassNumber)),
                AbsenceStudentCount = g.Count()
            });

        (int? shiftId, string? shiftName, int[] shiftHours)[] currentShifts;

        currentShifts = shifts
            .Select(cs => (
                shiftId: cs.ShiftId,
                shiftName: (string?)cs.ShiftName,
                shiftHours: cs.ShiftHours))
            .ToArray();

        if (isUnited)
        {
            var allShiftsHourNumbers = new List<int>();

            foreach (var shift in currentShifts)
            {
                allShiftsHourNumbers.AddRange(shift.shiftHours);
            }

            (int? shiftId, string? shiftName, int[] shiftHours) unitedShift =
                (shiftId: null, shiftName: null, shiftHours: allShiftsHourNumbers.Distinct().ToArray());

            currentShifts = new (int? shiftId, string? shiftName, int[] ShiftHours)[] { unitedShift };
        }

        var result = new List<GetItemsForAddVO>();

        foreach (var cb in allClassBooksStudents.Select(x => new { x.BookType, x.ClassBookId, x.ClassBookName }).Distinct())
        {
            foreach (var shift in currentShifts)
            {
                foreach (var hourNumber in shift.shiftHours)
                {
                    string? studentNumbers = null;
                    int absencesCount = 0;
                    if (cb.BookType != ClassBookType.Book_PG)
                    {
                        var currentAbsences = groupedAbsences.GetValueOrDefault(new { cb.ClassBookId, ShiftId = shift.shiftId, HourNumber = hourNumber });
                        if (currentAbsences != null)
                        {
                            studentNumbers = currentAbsences.AbsenceStudentNumbers;
                            absencesCount = currentAbsences.AbsenceStudentCount;
                        }
                    }
                    else
                    {
                        var currentAttendance = groupedAttendances.GetValueOrDefault(new { cb.ClassBookId, ShiftId = shift.shiftId });
                        if (currentAttendance != null)
                        {
                            studentNumbers = currentAttendance.AbsenceStudentNumbers;
                            absencesCount = currentAttendance.AbsenceStudentCount;
                        }
                    }

                    result.Add(new GetItemsForAddVO(
                        cb.ClassBookId,
                        cb.ClassBookName,
                        shift.shiftId,
                        shift.shiftName,
                        hourNumber,
                        studentNumbers,
                        absencesCount,
                        offDays.Contains(cb.ClassBookId),
                        classBooksWithSchedule.Contains(cb.ClassBookId)
                    ));
                }
            }
        }

        return result.ToArray();
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<DateAbsencesReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.DateAbsencesReportId == dateAbsencesReportId
            select mtr.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
        CancellationToken ct)
    {
        var items = (await (
            from ri in this.DbContext.Set<DateAbsencesReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.DateAbsencesReportId == dateAbsencesReportId

            orderby ri.DateAbsencesReportItemId

            select new
            {
                ri.DateAbsencesReportItemId,
                ri.ClassBookId,
                ri.ClassBookName,
                ri.ShiftId,
                ri.ShiftName,
                ri.HourNumber,
                ri.AbsenceStudentNumbers,
                ri.AbsenceStudentCount,
                ri.IsOffDay,
                ri.HasScheduleDate
            }
        ).ToArrayAsync(ct))
        .GroupBy(g => new
        {
            g.ClassBookId,
            g.ClassBookName,
            g.IsOffDay,
            g.HasScheduleDate,
        })
        .Select(g => new GetExcelDataVOItem(
            g.Key.ClassBookId,
            g.Key.ClassBookName,
            g.Key.IsOffDay,
            g.Key.HasScheduleDate,
            g.OrderBy(r => r.DateAbsencesReportItemId)
            .Select(c => new GetExcelDataVOClassItemHour(c.ShiftId, c.ShiftName, c.HourNumber, c.AbsenceStudentNumbers, c.AbsenceStudentCount))
            .ToArray()
        ))
        .ToArray();

        return await (
            from r in this.DbContext.Set<DateAbsencesReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.DateAbsencesReportId == dateAbsencesReportId

            select new GetExcelDataVO
            (
                r.ReportDate,
                r.IsUnited,
                r.ClassBookNames,
                r.ShiftNames,
                r.CreateDate,
                items
            )
        ).SingleAsync(ct);
    }
}
