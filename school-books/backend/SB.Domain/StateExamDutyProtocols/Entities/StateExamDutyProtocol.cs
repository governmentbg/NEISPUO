namespace SB.Domain;

using System;
using System.Collections.Generic;

public class StateExamDutyProtocol : IAggregateRoot
{
    // EF constructor
    private StateExamDutyProtocol()
    {
        this.Version = null!;
        this.OrderNumber = null!;
    }

    public StateExamDutyProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        int subjectId,
        int subjectTypeId,
        int? eduFormId,
        DateTime date,
        string orderNumber,
        DateTime orderDate,
        int modulesCount,
        string? roomNumber,
        int[] supervisorPersonIds,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.EduFormId = eduFormId;
        this.Date = date;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.ModulesCount = modulesCount;
        this.RoomNumber = roomNumber;

        this.SetSupervisors(supervisorPersonIds);

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }


    public int SchoolYear { get; private set; }

    public int StateExamDutyProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public string? SessionType { get; private set; }

    public int SubjectId { get; private set; }

    public int SubjectTypeId { get; private set; }

    public int? EduFormId { get; private set; }

    public DateTime Date { get; private set; }

    public string OrderNumber { get; private set; }

    public DateTime OrderDate { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int ModulesCount { get; private set; }

    public string? RoomNumber { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<StateExamDutyProtocolSupervisor> supervisors = new();
    public IReadOnlyCollection<StateExamDutyProtocolSupervisor> Supervisors => this.supervisors.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        int subjectId,
        int subjectTypeId,
        int? eduFormId,
        DateTime date,
        string orderNumber,
        DateTime orderDate,
        int modulesCount,
        string? roomNumber,
        int[] supervisorPersonIds,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.EduFormId = eduFormId;
        this.Date = date;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.ModulesCount = modulesCount;
        this.RoomNumber = roomNumber;

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
            this.supervisors.Add(new StateExamDutyProtocolSupervisor(this, personId));
        }
    }
}
