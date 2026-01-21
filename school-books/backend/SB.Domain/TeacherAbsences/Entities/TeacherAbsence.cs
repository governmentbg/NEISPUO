namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class TeacherAbsence : IAggregateRoot
{
    // EF constructor
    private TeacherAbsence()
    {
        this.Reason = null!;
    }

    public TeacherAbsence(
        int schoolYear,
        int instId,
        int teacherPersonId,
        DateTime startDate,
        DateTime endDate,
        string reason,
        (int scheduleLessonId, int? replTeacherPersonId, bool? replTeacherIsNonSpecialist, string? extReplTeacherName)[] hours,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.TeacherPersonId = teacherPersonId;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.Reason = reason;

        this.SetHours(hours);

        this.EnsureHoursNotIncludeAbsenteeTeacher();

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int TeacherAbsenceId { get; private set; }

    public int InstId { get; private set; }

    public int TeacherPersonId { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public string Reason { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<TeacherAbsenceHour> hours = new();
    public IReadOnlyCollection<TeacherAbsenceHour> Hours => this.hours.AsReadOnly();

    public bool ContainsPerson(int personId)
    {
        return this.TeacherPersonId == personId || this.Hours.Any(h => h.ReplTeacherPersonId == personId);
    }

    internal void UpdateData(
        string reason,
        (int scheduleLessonId, int? replTeacherPersonId, bool? replTeacherIsNonSpecialist, string? extReplTeacherName)[] hours,
        int modifiedBySysUserId)
    {
        this.Reason = reason;

        this.SetHours(hours);

        this.EnsureHoursNotIncludeAbsenteeTeacher();

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    private void SetHours((int scheduleLessonId, int? replTeacherPersonId, bool? replTeacherIsNonSpecialist, string? ExtReplTeacherName)[] hours)
    {
        if (hours == null)
        {
            throw new ArgumentNullException(nameof(hours));
        }

        if (hours.Length == 0)
        {
            throw new ArgumentException("At least one hour should be added to the array", nameof(hours));
        }

        this.hours.Clear();

        foreach (var hour in hours)
        {
            this.hours.Add(new TeacherAbsenceHour(
                this,
                hour.scheduleLessonId,
                hour.replTeacherPersonId,
                hour.replTeacherIsNonSpecialist,
                hour.ExtReplTeacherName));
        }
    }

    private void EnsureHoursNotIncludeAbsenteeTeacher()
    {
        if (this.Hours.Where(h => h.ReplTeacherPersonId == this.TeacherPersonId).Any())
        {
            throw new DomainException("The teacher absence hours must not include absentee teacher.");
        }
    }
}
