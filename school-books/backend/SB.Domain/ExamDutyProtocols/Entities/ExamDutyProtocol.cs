namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class ExamDutyProtocol : IAggregateRoot
{
    // EF constructor
    private ExamDutyProtocol()
    {
        this.Version = null!;
        this.OrderNumber = null!;
    }

    public ExamDutyProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        int subjectId,
        int subjectTypeId,
        int? eduFormId,
        int protocolExamTypeId,
        int protocolExamSubTypeId,
        DateTime date,
        string orderNumber,
        DateTime orderDate,
        string? groupNum,
        int[] classIds,
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
        this.ProtocolExamTypeId = protocolExamTypeId;
        this.ProtocolExamSubTypeId = protocolExamSubTypeId;
        this.Date = date;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.GroupNum = groupNum;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.SetClasses(classIds);
        this.SetSupervisors(supervisorPersonIds);
    }


    public int SchoolYear { get; private set; }

    public int ExamDutyProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public string? SessionType { get; private set; }

    public int SubjectId { get; private set; }

    public int SubjectTypeId { get; private set; }

    public int ProtocolExamTypeId { get; private set; }

    public int ProtocolExamSubTypeId { get; private set; }

    public string? GroupNum { get; private set; }

    public int? EduFormId { get; private set; }

    public DateTime Date { get; private set; }

    public string OrderNumber { get; private set; }

    public DateTime OrderDate { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<ExamDutyProtocolClass> classes = new();
    public IReadOnlyCollection<ExamDutyProtocolClass> Classes => this.classes.AsReadOnly();

    private readonly List<ExamDutyProtocolStudent> students = new();
    public IReadOnlyCollection<ExamDutyProtocolStudent> Students => this.students.AsReadOnly();

    private readonly List<ExamDutyProtocolSupervisor> supervisors = new();
    public IReadOnlyCollection<ExamDutyProtocolSupervisor> Supervisors => this.supervisors.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        int subjectId,
        int subjectTypeId,
        int? eduFormId,
        int protocolExamTypeId,
        int protocolExamSubTypeId,
        DateTime date,
        string orderNumber,
        string? groupNum,
        DateTime orderDate,
        int[] classIds,
        int[] supervisorPersonIds,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.EduFormId = eduFormId;
        this.ProtocolExamTypeId = protocolExamTypeId;
        this.ProtocolExamSubTypeId = protocolExamSubTypeId;
        this.Date = date;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.GroupNum = groupNum;

        this.SetClasses(classIds);
        this.SetSupervisors(supervisorPersonIds);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public ExamDutyProtocolStudent AddStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = new ExamDutyProtocolStudent(
            this,
            classId,
            personId);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public ExamDutyProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);

        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    private void SetClasses(int[] classIds)
    {
        this.classes.Clear();

        foreach (var classId in classIds)
        {
            this.classes.Add(new ExamDutyProtocolClass(this, classId));
        }
    }

    private void SetSupervisors(int[] personIds)
    {
        this.supervisors.Clear();

        foreach (var personId in personIds)
        {
            this.supervisors.Add(new ExamDutyProtocolSupervisor(this, personId));
        }
    }
}
