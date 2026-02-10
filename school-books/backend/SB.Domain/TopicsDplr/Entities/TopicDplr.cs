namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class TopicDplr : IAggregateRoot
{
    // EF constructor
    private TopicDplr()
    {
        this.Title = null!;
    }

    public TopicDplr(
        int schoolYear,
        int classBookId,
        DateTime date,
        int day,
        int hourNumber,
        TimeSpan startTime,
        TimeSpan endTime,
        int curriculumId,
        string? location,
        string title,
        int[] teachers,
        int[]? students,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Date = date;
        this.Day = day;
        this.HourNumber = hourNumber;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.CurriculumId = curriculumId;
        this.Location = location;
        this.Title = title;

        this.teachers.AddRange(teachers.Select(t => new TopicDplrTeacher(this, t)));

        if (students != null && students.Any())
        {
            this.students.AddRange(students.Select(s => new TopicDplrStudent(this, s)));
        }

        this.IsVerified = false;
        this.CreateDate = DateTime.Now;
        this.CreatedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int TopicDplrId { get; private set; }

    public int ClassBookId { get; private set; }

    public DateTime Date { get; private set; }

    public int Day { get; init; }

    public int HourNumber { get; init; }

    public TimeSpan StartTime { get; init; }

    public TimeSpan EndTime { get; init; }

    public int CurriculumId { get; private set; }

    public string? Location { get; init; }

    public string Title { get; private set; }

    public DateTime CreateDate { get; private set; }

    public bool IsVerified { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime? VerifyDate { get; private set; }

    public int? VerifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<TopicDplrTeacher> teachers = new();
    public IReadOnlyCollection<TopicDplrTeacher> Teachers => this.teachers.AsReadOnly();

    private readonly List<TopicDplrStudent> students = new();
    public IReadOnlyCollection<TopicDplrStudent> Students => this.students.AsReadOnly();

    public void UpdateIsVerified(
        bool isVerified,
        int verifiedBySysUserId)
    {
        this.IsVerified = isVerified;

        this.VerifyDate = DateTime.Now;
        this.VerifiedBySysUserId = verifiedBySysUserId;
    }
}
