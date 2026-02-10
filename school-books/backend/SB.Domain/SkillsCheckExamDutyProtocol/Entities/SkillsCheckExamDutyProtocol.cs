namespace SB.Domain;

using System;
using System.Collections.Generic;

public class SkillsCheckExamDutyProtocol : IAggregateRoot
{
    // EF constructor
    private SkillsCheckExamDutyProtocol()
    {
        this.Version = null!;
    }

    public SkillsCheckExamDutyProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        DateTime? protocolDate,
        int subjectId,
        int subjectTypeId,
        DateTime date,
        int directorPersonId,
        int studentsCapacity,
        int[] supervisorPersonIds,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.Date = date;
        this.DirectorPersonId = directorPersonId;
        this.StudentsCapacity = studentsCapacity;

        this.SetSupervisors(supervisorPersonIds);

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }


    public int SchoolYear { get; private set; }

    public int SkillsCheckExamDutyProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public int SubjectId { get; private set; }

    public int SubjectTypeId { get; private set; }

    public DateTime Date { get; private set; }

    public int StudentsCapacity { get; private set; }

    public int DirectorPersonId { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<SkillsCheckExamDutyProtocolSupervisor> supervisors = new();
    public IReadOnlyCollection<SkillsCheckExamDutyProtocolSupervisor> Supervisors => this.supervisors.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        int subjectId,
        int subjectTypeId,
        DateTime date,
        int directorPersonId,
        int studentsCapacity,
        int[] supervisorPersonIds,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.Date = date;
        this.DirectorPersonId = directorPersonId;
        this.StudentsCapacity = studentsCapacity;

        this.SetSupervisors(supervisorPersonIds);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    private void SetSupervisors(int[] personIds)
    {
        if (personIds.Length == 0)
        {
            throw new ArgumentException("At least one supervisor should be added to the array", nameof(personIds));
        }

        this.supervisors.Clear();

        foreach (var personId in personIds)
        {
            this.supervisors.Add(new SkillsCheckExamDutyProtocolSupervisor(this, personId));
        }
    }
}
