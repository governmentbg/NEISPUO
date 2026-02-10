namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class NvoExamDutyProtocol : IAggregateRoot
{
    // EF constructor
    private NvoExamDutyProtocol()
    {
        this.Version = null!;
    }

    public NvoExamDutyProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        DateTime? protocolDate,
        int basicClassId,
        int subjectId,
        int subjectTypeId,
        DateTime date,
        string? roomNumber,
        int directorPersonId,
        int[] supervisorPersonIds,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.BasicClassId = basicClassId;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.Date = date;
        this.RoomNumber = roomNumber;
        this.DirectorPersonId = directorPersonId;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.SetSupervisors(supervisorPersonIds);
    }


    public int SchoolYear { get; private set; }

    public int NvoExamDutyProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public int BasicClassId { get; private set; }

    public int SubjectId { get; private set; }

    public int SubjectTypeId { get; private set; }

    public DateTime Date { get; private set; }

    public string? RoomNumber { get; private set; }

    public int DirectorPersonId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations

    private readonly List<NvoExamDutyProtocolStudent> students = new();
    public IReadOnlyCollection<NvoExamDutyProtocolStudent> Students => this.students.AsReadOnly();

    private readonly List<NvoExamDutyProtocolSupervisor> supervisors = new();
    public IReadOnlyCollection<NvoExamDutyProtocolSupervisor> Supervisors => this.supervisors.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        int basicClassId,
        int subjectId,
        int subjectTypeId,
        DateTime date,
        string? roomNumber,
        int directorPersonId,
        int[] supervisorPersonIds,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.BasicClassId = basicClassId;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.Date = date;
        this.RoomNumber = roomNumber;
        this.DirectorPersonId = directorPersonId;

        this.SetSupervisors(supervisorPersonIds);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public NvoExamDutyProtocolStudent AddStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = new NvoExamDutyProtocolStudent(
            this,
            classId,
            personId);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public NvoExamDutyProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);

        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
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
            this.supervisors.Add(new NvoExamDutyProtocolSupervisor(this, personId));
        }
    }
}
