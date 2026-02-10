namespace SB.Domain;

using System;

public class Performance : IAggregateRoot
{
    // EF constructor
    private Performance()
    {
        this.Name = null!;
        this.Description = null!;
        this.Location = null!;
        this.Version = null!;
    }

    public Performance(
        int schoolYear,
        int classBookId,
        int performanceTypeId,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        string location,
        string? studentAwards,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PerformanceTypeId = performanceTypeId;
        this.Name = name;
        this.Description = description;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.Location = location;
        this.StudentAwards = studentAwards;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int PerformanceId { get; private set; }
    public int ClassBookId { get; private set; }
    public int PerformanceTypeId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string Location { get; private set; }
    public string? StudentAwards { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void UpdateData(
        int performanceTypeId,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        string location,
        string? studentAwards,
        int modifiedBySysUserId)
    {
        this.PerformanceTypeId = performanceTypeId;
        this.Name = name;
        this.Description = description;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.Location = location;
        this.StudentAwards = studentAwards;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
