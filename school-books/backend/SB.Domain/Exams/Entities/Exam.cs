namespace SB.Domain;

using System;

public class Exam : IAggregateRoot
{
    // EF constructor
    private Exam()
    {
        this.Version = null!;
    }

    public Exam(
        int schoolYear,
        int classBookId,
        BookExamType type,
        int curriculumId,
        DateTime date,
        string? description,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Type = type;
        this.CurriculumId = curriculumId;
        this.Date = date;
        this.Description = description;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }


    public int SchoolYear { get; private set; }

    public int ExamId { get; private set; }

    public int ClassBookId { get; private set; }

    public BookExamType Type { get; private set; }

    public int CurriculumId { get; private set; }

    public DateTime Date { get; private set; }

    public string? Description { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    public void UpdateData(
        DateTime date,
        string? description,
        int modifiedBySysUserId)
    {
        this.Date = date;
        this.Description = description;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void ExtUpdateData(
        int curriculumId,
        DateTime date,
        string? description,
        int modifiedBySysUserId)
    {
        this.CurriculumId = curriculumId;
        this.Date = date;
        this.Description = description;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
