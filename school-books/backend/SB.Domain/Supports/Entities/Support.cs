namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class Support : IAggregateRoot
{
    // EF constructor
    private Support()
    {
        this.Version = null!;
    }

    public Support(
        int schoolYear,
        int classBookId,
        DateTime endDate,
        string? description,
        string? expectedResult,
        int createdBySysUserId,
        int[] studentIds,
        int[] teacherIds,
        int[] supportDifficultyTypeIds)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.EndDate = endDate;
        this.Description = description;
        this.ExpectedResult = expectedResult;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.IsForAllStudents = !studentIds.Any();
        this.students.AddRange(studentIds.Select(id => new SupportStudent(this, id)));
        this.teachers.AddRange(teacherIds.Select(id => new SupportTeacher(this, id)));
        this.difficultyTypes.AddRange(supportDifficultyTypeIds.Select(typeId => new SupportDifficulty(this, typeId)));
    }

    public int SchoolYear { get; private set; }

    public int SupportId { get; private set; }

    public int ClassBookId { get; private set; }

    public DateTime EndDate { get; private set; }

    public string? Description { get; private set; }

    public string? ExpectedResult { get; private set; }

    public bool IsForAllStudents { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<SupportActivity> activities = new();
    public IReadOnlyCollection<SupportActivity> Activities => this.activities.AsReadOnly();

    private readonly List<SupportDifficulty> difficultyTypes = new();
    public IReadOnlyCollection<SupportDifficulty> DifficultyTypes => this.difficultyTypes.AsReadOnly();

    private readonly List<SupportTeacher> teachers = new();
    public IReadOnlyCollection<SupportTeacher> Teachers => this.teachers.AsReadOnly();

    private readonly List<SupportStudent> students = new();
    public IReadOnlyCollection<SupportStudent> Students => this.students.AsReadOnly();

    public void Update(
        DateTime endDate,
        string? description,
        string? expectedResult,
        int[] studentIds,
        int[] teacherIds,
        int[] supportDifficultyTypeIds,
        int modifiedBySysUserId)
    {
        this.EndDate = endDate;
        this.Description = description;
        this.ExpectedResult = expectedResult;

        this.IsForAllStudents = !studentIds.Any();

        this.students.Clear();
        this.students.AddRange(studentIds.Select(id => new SupportStudent(this, id)));

        if (teacherIds == null || teacherIds.Length == 0)
        {
            throw new DomainValidationException($"{nameof(Support)} should have at lease one teacher");
        }

        this.teachers.Clear();
        this.teachers.AddRange(teacherIds.Select(id => new SupportTeacher(this, id)));

        if (supportDifficultyTypeIds == null || supportDifficultyTypeIds.Length == 0)
        {
            throw new DomainValidationException($"{nameof(Support)} should have at lease one difficulty type");
        }

        this.difficultyTypes.Clear();
        this.difficultyTypes.AddRange(supportDifficultyTypeIds.Select(typeId => new SupportDifficulty(this, typeId)));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SupportActivity AddActivity(
        int supportActivityTypeId,
        string? target,
        string? result,
        DateTime? date,
        int modifiedBySysUserId)
    {
        var activity = new SupportActivity(
            this,
            supportActivityTypeId,
            target,
            result,
            date);

        this.activities.Add(activity);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return activity;
    }

    public void UpdateActivity(
        int supportActivityId,
        int supportActivityTypeId,
        string? target,
        string? result,
        DateTime? date,
        int modifiedBySysUserId)
    {
        var activity = this.activities.Single(i => i.SupportActivityId == supportActivityId);
        activity.UpdateData(
            supportActivityTypeId,
            target,
            result,
            date);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SupportActivity RemoveActivity(int supportActivityId, int modifiedBySysUserId)
    {
        var activity = this.Activities.Single(i => i.SupportActivityId == supportActivityId);
        this.activities.Remove(activity);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return activity;
    }

    public void UpdateExternal(
        DateTime endDate,
        string? description,
        string? expectedResult,
        int[] studentIds,
        int[] teacherIds,
        int[] supportDifficultyTypeIds,
        (int supportActivityTypeId, string? target, string? result, DateTime? date)[] activities,
        int modifiedBySysUserId)
    {
        this.EndDate = endDate;
        this.Description = description;
        this.ExpectedResult = expectedResult;

        this.IsForAllStudents = !studentIds.Any();

        this.students.Clear();
        this.students.AddRange(studentIds.Select(id => new SupportStudent(this, id)));

        if (teacherIds == null || teacherIds.Length == 0)
        {
            throw new DomainValidationException($"{nameof(Support)} should have at lease one teacher");
        }

        this.teachers.Clear();
        this.teachers.AddRange(teacherIds.Select(id => new SupportTeacher(this, id)));

        if (supportDifficultyTypeIds == null || supportDifficultyTypeIds.Length == 0)
        {
            throw new DomainValidationException($"{nameof(Support)} should have at lease one difficulty type");
        }

        this.difficultyTypes.Clear();
        this.difficultyTypes.AddRange(supportDifficultyTypeIds.Select(typeId => new SupportDifficulty(this, typeId)));

        this.activities.Clear();
        this.activities.AddRange(activities.Select(a => new SupportActivity(this, a.supportActivityTypeId, a.target, a.result, a.date)));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
