namespace SB.Domain;

using SB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Schedule : IAggregateRoot
{
    public static readonly int[] ScheduleIsoDays = new int[] { 1, 2, 3, 4, 5, 6, 7 };

    // EF constructor
    private Schedule()
    {
    }

    public Schedule(
        int schoolYear,
        int classBookId,
        bool isIndividualSchedule,
        int? personId,
        SchoolTerm? term,
        DateTime startDate,
        DateTime endDate,
        int shiftId,
        DateTime[] dates,
        DateTime[] offDayDates,
        (int day, int hourNumber, int curriculumId, string? location)[] hours,
        bool includesWeekend,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.IsIndividualSchedule = isIndividualSchedule;
        this.PersonId = personId;
        this.Term = term;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.ShiftId = shiftId;
        this.IsRziApproved = false;
        this.IncludesWeekend = includesWeekend;
        this.IsSplitting = false;

        if (this.StartDate > this.EndDate)
        {
            throw new DomainValidationException("StartDate must be less than or equal to EndDate");
        }

        if ((!isIndividualSchedule && personId != null) ||
            (isIndividualSchedule && personId == null))
        {
            throw new DomainValidationException("Schedule personId is only allowed when isIndividualSchedule is true");
        }

        this.SetDates(dates);
        this.SetHours(hours);
        this.SetLessons(dates, offDayDates, hours.Select(h => (h.day, h.hourNumber, h.curriculumId, h.location)).ToArray());

        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifiedBySysUserId = createdBySysUserId;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
    }

    // split schedule constructor
    private Schedule(
        Schedule originalSchedule,
        DateTime[] splitDates,
        Shift? clonedAdhocShift,
        int createdBySysUserId)
    {
        var sortedSplitDates =
            splitDates
            .OrderBy(d => d)
            .ToArray();

        this.SchoolYear = originalSchedule.SchoolYear;
        this.ClassBookId = originalSchedule.ClassBookId;
        this.IsIndividualSchedule = originalSchedule.IsIndividualSchedule;
        this.PersonId = originalSchedule.PersonId;
        this.Term = originalSchedule.Term;
        this.StartDate = sortedSplitDates.First().Date;
        this.EndDate = sortedSplitDates.Last().Date;
        this.ShiftId = clonedAdhocShift != null ? clonedAdhocShift.ShiftId : originalSchedule.ShiftId;
        this.IsRziApproved = false;
        this.IncludesWeekend = originalSchedule.IncludesWeekend;
        this.IsSplitting = true;

        this.SetDates(splitDates);
        this.SetHours(originalSchedule.Hours
            .Select(h => (day: h.Day, hourNumber: h.HourNumber, curriculumId: h.CurriculumId, location: h.Location)).ToArray());

        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifiedBySysUserId = createdBySysUserId;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
    }

    public int SchoolYear { get; private set; }

    public int ScheduleId { get; private set; }

    public int ClassBookId { get; private set; }

    public bool IsIndividualSchedule { get; private set; }

    public int? PersonId { get; private set; }

    public SchoolTerm? Term { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public int ShiftId { get; private set; }

    public bool IsRziApproved { get; private set; }

    public bool IncludesWeekend { get; private set; }

    public bool IsSplitting { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<ScheduleDate> dates = new();
    public IReadOnlyCollection<ScheduleDate> Dates => this.dates.AsReadOnly();

    private readonly List<ScheduleHour> hours = new();
    public IReadOnlyCollection<ScheduleHour> Hours => this.hours.AsReadOnly();

    private readonly List<ScheduleLesson> lessons = new();
    public IReadOnlyCollection<ScheduleLesson> Lessons => this.lessons.AsReadOnly();

    public void UpdateData(
        SchoolTerm? term,
        DateTime startDate,
        DateTime endDate,
        bool includesWeekend,
        int shiftId,
        DateTime[] dates,
        DateTime[] offDayDates,
        (int day, int hourNumber, int curriculumId, string? location)[] hours,
        int modifiedBySysUserId)
    {
        this.Term = term;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.IncludesWeekend = includesWeekend;
        this.ShiftId = shiftId;

        this.SetDates(dates);
        this.SetHours(hours);
        this.SetLessons(dates, offDayDates, hours.Select(h => (h.day, h.hourNumber, h.curriculumId, h.location)).ToArray());

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public Schedule SplitSchedule(DateTime[] datesToMoveToNewSchedule, Shift? clonedAdhocShift, int modifiedBySysUserId)
    {
        if (!new HashSet<DateTime>(this.Dates.Select(d => d.Date)).IsSupersetOf(datesToMoveToNewSchedule))
        {
            throw new DomainValidationException("The dates to move to the new schedule must be a subset of the schedule dates");
        }

        var datesToMoveToNewScheduleDict = datesToMoveToNewSchedule.ToDictionary(d => d);

        var newDates = this.Dates
            .Where(d => !datesToMoveToNewScheduleDict.ContainsKey(d.Date))
            .Select(d => d.Date)
            .OrderBy(d => d)
            .ToArray();

        if (newDates.Length == 0)
        {
            throw new DomainValidationException("There must be dates left in the schedule after the split");
        }

        // update original schedule
        this.StartDate = newDates.First().Date;
        this.EndDate = newDates.Last().Date;
        this.SetDates(newDates);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        // create new schedule
        var splitSchedule = new Schedule(
            this,
            datesToMoveToNewSchedule,
            clonedAdhocShift,
            modifiedBySysUserId);

        // move lessons to new schedule
        var lessonsToMoveToNewSchedule =
            this.lessons.RemoveAllMatching(l => datesToMoveToNewScheduleDict.ContainsKey(l.Date));
        splitSchedule.lessons.AddRange(lessonsToMoveToNewSchedule);

        return splitSchedule;
    }

    public void UpdateIsRziApproved(bool isRziApproved, int modifiedBySysUserId)
    {
        this.IsRziApproved = isRziApproved;
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void FinalizeSplitting()
    {
        this.IsSplitting = false;
    }

    public void AddLessonsForDate(DateTime date)
    {
        var newLessons = new List<ScheduleLesson>();

        if (!this.lessons.Any(l => l.Date == date) && this.dates.Any(d => d.Date == date))
        {
            foreach (var hour in this.hours.Where(h => h.Day == DateExtensions.GetIsoDayOfWeek(date)))
            {
                newLessons.Add(new ScheduleLesson(this, date, hour.Day, hour.HourNumber, hour.CurriculumId));
            }
        }

        this.lessons.AddRange(newLessons);
    }

    private void SetDates(DateTime[] dates)
    {
        if (dates.Length == 0)
        {
            throw new ArgumentException("At least one date should exist", nameof(dates));
        }

        var oldDates = new HashSet<DateTime>(this.dates.Select(d => d.Date));
        var newDates = new HashSet<DateTime>(dates);

        this.dates.RemoveAll(d => !newDates.Contains(d.Date));
        this.dates.AddRange(newDates.Where(d => !oldDates.Contains(d)).Select(d => new ScheduleDate(this, d)));
    }

    private void SetHours((int day, int hourNumber, int curriculumId, string? location)[] hours)
    {
        if (hours.Length == 0)
        {
            throw new ArgumentException("At least one hour should exist", nameof(hours));
        }

        var oldHours = new HashSet<(int day, int hourNumber, int curriculumId, string? location)>(this.hours.Select(h => (h.Day, h.HourNumber, h.CurriculumId, h.Location)));
        var newHours = new HashSet<(int day, int hourNumber, int curriculumId, string? location)>(hours);

        this.hours.RemoveAll(h => !newHours.Contains((h.Day, h.HourNumber, h.CurriculumId, h.Location)));
        this.hours.AddRange(
            newHours.Where(h => !oldHours.Contains((h.day, h.hourNumber, h.curriculumId, h.location)))
            .Select(h => new ScheduleHour(this, h.day, h.hourNumber, h.curriculumId, h.location)));
    }

    private void SetLessons(DateTime[] dates, DateTime[] offDayDates, (int day, int hourNumber, int curriculumId, string? ocation)[] hours)
    {
        if (dates.Length == 0)
        {
            throw new ArgumentException("At least one date should exist", nameof(dates));
        }

        if (hours.Length == 0)
        {
            throw new ArgumentException("At least one hour should exist", nameof(hours));
        }

        // For ease of use(a single column for foreign keys) the lesson has its own id even though its
        // identity is entirely dependent on all of its properties.

        // In theory we could clear/recreate the lessons collection on every dates/hours change but this will:
        // 1. Throw "The DELETE statement conflicted with the REFERENCE constraint",
        //  as the lesson is referenced in Grades, Absences, etc.
        // 2. Be very inefficient

        // Instead we will create a new collection with the existing and newly created lessons.
        // Those that are missing need to and will be removed.

        var dayHoursLookup = hours.ToLookup(h => h.day);

        var existingHoursDict = this.hours.ToDictionary(
            l => (day: l.Day, hourNumber: l.HourNumber, curriculumId: l.CurriculumId, location: l.Location));

        var existingLessonsDict = this.lessons.ToDictionary(
            l => (date: l.Date, day: l.Day, hourNumber: l.HourNumber, curriculumId: l.CurriculumId));

        var keepLessons = new HashSet<int>();
        var newLessons = new List<ScheduleLesson>();
        foreach (var date in dates.Where(d => !offDayDates.Contains(d)))
        {
            foreach (var (day, hourNumber, curriculumId, location) in dayHoursLookup[DateExtensions.GetIsoDayOfWeek(date)])
            {
                existingLessonsDict.TryGetValue((date, day, hourNumber, curriculumId), out var lesson);

                var changedLocation = !(existingHoursDict.TryGetValue((day, hourNumber, curriculumId, location), out var hour) && hour?.ScheduleId != 0);

                if (lesson != null && !changedLocation)
                {
                    keepLessons.Add(lesson.ScheduleLessonId);
                }
                else
                {
                    newLessons.Add(new ScheduleLesson(this, date, day, hourNumber, curriculumId));
                }
            }
        }

        this.lessons.RemoveAll(l => !keepLessons.Contains(l.ScheduleLessonId));
        this.lessons.AddRange(newLessons);
    }

    public static async Task<DateTime[]> CalcDatesAsync(
        int schoolYear,
        int classBookId,
        bool isIndividualSchedule,
        int? personId,
        int? scheduleId,
        DateTime startDate,
        DateTime endDate,
        ScheduleCommandWeek[] weeks,
        int[] usedDays,
        ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
    {
        var scheduleDates = new List<DateTime>();
        var usedDates = (
            await schedulesQueryRepository.GetUsedDatesAsync(
                schoolYear,
                classBookId,
                isIndividualSchedule,
                personId,
                scheduleId,
                ct)
        ).ToHashSet();

        foreach (var (year, weekNumber) in weeks)
        {
            var weekStart = DateExtensions.GetDateFromIsoWeek(year, weekNumber, Schedule.ScheduleIsoDays[0]);
            var weekEnd = DateExtensions.GetDateFromIsoWeek(year, weekNumber, Schedule.ScheduleIsoDays[^1]);

            scheduleDates.AddRange(
                DateExtensions.GetDatesInRange(
                    DateExtensions.Max(weekStart, startDate),
                    DateExtensions.Min(weekEnd, endDate)
                )
                .Where(d =>
                    usedDays.Contains(d.GetIsoDayOfWeek()) && // skip empty days
                    !usedDates.Contains(d)) // skip dates from other schedules
            );
        }

        return scheduleDates.ToArray();
    }

    public Schedule CopyScheduleToNewClassBook(int newClassBookId, DateTime[] offDays, int? shiftId)
    {
        return new Schedule(
            this.SchoolYear,
            newClassBookId,
            this.IsIndividualSchedule,
            this.PersonId,
            this.Term,
            this.StartDate,
            this.EndDate,
            shiftId ?? this.ShiftId,
            this.dates.Select(d => d.Date).ToArray(),
            offDays,
            this.Hours.Select(h => (day: h.Day, hourNumber: h.HourNumber, curriculumId: h.CurriculumId, location: h.Location)).ToArray(),
            this.IncludesWeekend,
            this.CreatedBySysUserId);
    }
}
