namespace SB.Domain;

using System;

public class ClassBookTopicPlanItem : IAggregateRoot
{
    // EF constructor
    private ClassBookTopicPlanItem()
    {
        this.Version = null!;
        this.Title = null!;
    }

    public ClassBookTopicPlanItem(
        int schoolYear,
        int classBookId,
        int curriculumId,
        int number,
        string title,
        string? note,
        bool taken,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.CurriculumId = curriculumId;
        this.Number = number;
        this.Title = title;
        this.Note = note;
        this.Taken = taken;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int ClassBookTopicPlanItemId { get; private set; }
    public int ClassBookId { get; private set; }
    public int CurriculumId { get; private set; }
    public int Number { get; private set; }
    public string Title { get; private set; }
    public string? Note { get; private set; }
    public bool Taken { get; private set; }

    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void Update(
        int number,
        string title,
        string? note,
        bool taken,
        int modifiedBySysUserId)
    {
        this.Number = number;
        this.Title = title;
        this.Note = note;
        this.Taken = taken;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateTaken(
        bool taken,
        int modifiedBySysUserId)
    {
        this.Taken = taken;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
