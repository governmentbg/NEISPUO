namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class QualificationExamResultProtocol : IAggregateRoot
{
    // EF constructor
    private QualificationExamResultProtocol()
    {
        this.Version = null!;
        this.Profession = null!;
        this.Speciality = null!;
        this.CommissionNominationOrderNumber = null!;
    }

    public QualificationExamResultProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        string profession,
        string speciality,
        int qualificationDegreeId,
        string? groupNum,
        int[] classIds,
        int? eduFormId,
        int qualificationExamTypeId,
        DateTime date,
        string commissionNominationOrderNumber,
        DateTime commissionNominationOrderDate,
        int commissionChairman,
        int[] commissionMembers,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.Profession = profession;
        this.Speciality = speciality;
        this.QualificationDegreeId = qualificationDegreeId;
        this.GroupNum = groupNum;
        this.EduFormId = eduFormId;
        this.QualificationExamTypeId = qualificationExamTypeId;
        this.CommissionNominationOrderNumber = commissionNominationOrderNumber;
        this.CommissionNominationOrderDate = commissionNominationOrderDate;
        this.Date = date;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.SetClasses(classIds);
        this.SetCommissioners(commissionChairman, commissionMembers);
    }


    public int SchoolYear { get; private set; }

    public int QualificationExamResultProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public string? SessionType { get; private set; }

    public string Profession { get; private set; }

    public string Speciality { get; private set; }

    public int QualificationDegreeId { get; private set; }

    public string? GroupNum { get; private set; }

    public int QualificationExamTypeId { get; private set; }

    public int? EduFormId { get; private set; }

    public DateTime Date { get; private set; }

    public string CommissionNominationOrderNumber { get; private set; }

    public DateTime CommissionNominationOrderDate { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<QualificationExamResultProtocolClass> classes = new();
    public IReadOnlyCollection<QualificationExamResultProtocolClass> Classes => this.classes.AsReadOnly();

    private readonly List<QualificationExamResultProtocolStudent> students = new();
    public IReadOnlyCollection<QualificationExamResultProtocolStudent> Students => this.students.AsReadOnly();

    private readonly List<QualificationExamResultProtocolCommissioner> commissioners = new();
    public IReadOnlyCollection<QualificationExamResultProtocolCommissioner> Commissioners => this.commissioners.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        string profession,
        string speciality,
        int qualificationDegreeId,
        string? groupNum,
        int[] classIds,
        int? eduFormId,
        int qualificationExamTypeId,
        DateTime date,
        string commissionNominationOrderNumber,
        DateTime commissionNominationOrderDate,
        int commissionChairman,
        int[] commissionMembers,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.Profession = profession;
        this.Speciality = speciality;
        this.QualificationDegreeId = qualificationDegreeId;
        this.GroupNum = groupNum;
        this.EduFormId = eduFormId;
        this.QualificationExamTypeId = qualificationExamTypeId;
        this.CommissionNominationOrderNumber = commissionNominationOrderNumber;
        this.CommissionNominationOrderDate = commissionNominationOrderDate;
        this.Date = date;

        this.SetClasses(classIds);
        this.SetCommissioners(commissionChairman, commissionMembers);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    private void SetCommissioners(int commissionChairman, int[] commissionMembers)
    {
        if (commissionMembers == null)
        {
            throw new ArgumentNullException(nameof(commissionMembers));
        }

        this.commissioners.Clear();
        this.commissioners.Add(new QualificationExamResultProtocolCommissioner(this, commissionChairman, true, 0));
        this.commissioners.AddRange(commissionMembers.Select((id, index) => new QualificationExamResultProtocolCommissioner(this, id, false, index + 1)));
    }

    public QualificationExamResultProtocolStudent AddStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = new QualificationExamResultProtocolStudent(
            this,
            classId,
            personId);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public QualificationExamResultProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);

        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    private void SetClasses(int[] classIds)
    {
        if (classIds.Length == 0)
        {
            throw new ArgumentException("At least one class should be added to the array", nameof(classIds));
        }

        this.classes.Clear();

        foreach (var classId in classIds)
        {
            this.classes.Add(new QualificationExamResultProtocolClass(this, classId));
        }
    }
}
