namespace SB.Domain;

using System;

public class Remark : IAggregateRoot
{
    // EF constructor
    private Remark()
    {
        this.Description = null!;
        this.Version = null!;
    }

    public Remark(
        int schoolYear,
        int classBookId,
        int personId,
        RemarkType type,
        int curriculumId,
        DateTime date,
        string description,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.Type = type;
        this.CurriculumId = curriculumId;
        this.Date = date;
        this.Description = description;
        this.IsReadFromParent = false;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int RemarkId { get; private set; }
    public int ClassBookId { get; private set; }
    public int PersonId { get; private set; }
    public RemarkType Type { get; private set; }
    public int CurriculumId { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    public bool IsReadFromParent { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void UpdateData(
        DateTime date,
        string description,
        int modifiedBySysUserId)
    {
        this.Date = date;
        this.Description = description;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public string EmailTag
    {
        get
        {
            return $"remark:{this.SchoolYear}:{this.RemarkId}";
        }
    }

    public string PushNotificationTag
    {
        get
        {
            return $"remark-push:{this.SchoolYear}:{this.RemarkId}";
        }
    }
    public void SetAsReadFromParent()
    {
        this.IsReadFromParent = true;
    }
}
