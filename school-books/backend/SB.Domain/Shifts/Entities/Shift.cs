namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class Shift : IAggregateRoot
{
    // EF constructor
    private Shift()
    {
        this.Name = null!;
    }

    public Shift(
        int schoolYear,
        int instId,
        string name,
        bool isMultiday,
        bool isAdhoc,
        (int day, int hourNumber, string startTime, string endTime)[] presentationHours,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;

        this.Name = name;
        this.IsMultiday = isMultiday;
        this.IsAdhoc = isAdhoc;
        this.SetPresentationHours(isMultiday, presentationHours);

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int ShiftId { get; private set; }

    public int InstId { get; private set; }

    public string Name { get; private set; }

    public bool IsMultiday { get; private set; }

    public bool IsAdhoc { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<ShiftHour> hours = new();
    public IReadOnlyCollection<ShiftHour> Hours => this.hours.AsReadOnly();

    public void UpdateData(
        string name,
        bool isMultiday,
        (int day, int hourNumber, string startTime, string endTime)[] presentationHours,
        int modifiedBySysUserId)
    {
        this.Name = name;
        this.IsMultiday = isMultiday;
        this.SetPresentationHours(isMultiday, presentationHours);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public Shift CloneAdhocShift(int createdBySysUserId)
    {
        if (!this.IsAdhoc)
        {
            throw new DomainValidationException("Cannot clone non adhoc shift");
        }

        var now = DateTime.Now;

        var clone = new Shift()
        {
            SchoolYear = this.SchoolYear,
            InstId = this.InstId,
            Name = this.Name,
            IsMultiday = this.IsMultiday,
            IsAdhoc = this.IsAdhoc,
            CreateDate = now,
            CreatedBySysUserId = createdBySysUserId,
            ModifyDate = now,
            ModifiedBySysUserId = createdBySysUserId
        };

        clone.hours.AddRange(
            this.hours.Select(h => new ShiftHour(clone, h.Day, h.HourNumber, h.StartTime, h.EndTime)));

        return clone;
    }

    private void SetPresentationHours(bool isMultiday, (int day, int hourNumber, string startTime, string endTime)[] hours)
    {
        if (hours == null)
        {
            throw new ArgumentNullException(nameof(hours));
        }

        if (hours.Length == 0)
        {
            throw new ArgumentException("At least one hour should be added to the array", nameof(hours));
        }

        foreach (var day in hours.GroupBy(h => h.day))
        {
            foreach (var duplicateHour in day.GroupBy(h => h.hourNumber).Where(g => g.Count() > 1))
            {
                throw new DomainValidationException($"Cannot have duplicate hour numbers in a day - Day: {day.Key}, HourNumber: {duplicateHour.Key}");
            }
        }

        this.hours.Clear();

        if (!isMultiday)
        {
            if (!hours.All(h => h.day == 1))
            {
                throw new DomainValidationException("In a non multiday shift all presentation hours must be for day 1");
            }

            foreach (var (_, hourNumber, startTime, endTime) in hours)
            {
                var start = TimeSpan.Parse(startTime);
                var end = TimeSpan.Parse(endTime);

                for (var day = 1; day <= 7; day++)
                {
                    this.hours.Add(new ShiftHour(this, day, hourNumber, start, end));
                }
            }
        }
        else
        {
            foreach (var (day, hourNumber, startTime, endTime) in hours)
            {
                var start = TimeSpan.Parse(startTime);
                var end = TimeSpan.Parse(endTime);

                this.hours.Add(new ShiftHour(this, day, hourNumber, start, end));
            }
        }
    }
}
