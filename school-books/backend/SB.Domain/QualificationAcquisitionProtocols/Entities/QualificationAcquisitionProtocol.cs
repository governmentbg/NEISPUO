namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class QualificationAcquisitionProtocol : IAggregateRoot
{
    // EF constructor
    private QualificationAcquisitionProtocol()
    {
        this.Version = null!;
        this.Profession = null!;
        this.Speciality = null!;
        this.CommissionNominationOrderNumber = null!;
    }

    public QualificationAcquisitionProtocol(
        int schoolYear,
        int instId,
        QualificationAcquisitionProtocolType protocolType,
        string? protocolNumber,
        DateTime? protocolDate,
        string profession,
        string speciality,
        int qualificationDegreeId,
        DateTime date,
        string commissionNominationOrderNumber,
        DateTime commissionNominationOrderDate,
        int commissionChairman,
        int directorPersonId,
        int[] commissionMembers,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolType = protocolType;
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.Profession = profession;
        this.Speciality = speciality;
        this.QualificationDegreeId = qualificationDegreeId;
        this.CommissionNominationOrderNumber = commissionNominationOrderNumber;
        this.CommissionNominationOrderDate = commissionNominationOrderDate;
        this.DirectorPersonId = directorPersonId;
        this.Date = date;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.SetCommissioners(commissionChairman, commissionMembers);
    }


    public int SchoolYear { get; private set; }

    public int QualificationAcquisitionProtocolId { get; private set; }

    public int InstId { get; private set; }

    public QualificationAcquisitionProtocolType ProtocolType { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public string Profession { get; private set; }

    public string Speciality { get; private set; }

    public int QualificationDegreeId { get; private set; }

    public DateTime Date { get; private set; }

    public string CommissionNominationOrderNumber { get; private set; }

    public DateTime CommissionNominationOrderDate { get; private set; }

    public int DirectorPersonId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<QualificationAcquisitionProtocolStudent> students = new();
    public IReadOnlyCollection<QualificationAcquisitionProtocolStudent> Students => this.students.AsReadOnly();

    private readonly List<QualificationAcquisitionProtocolCommissioner> commissioners = new();
    public IReadOnlyCollection<QualificationAcquisitionProtocolCommissioner> Commissioners => this.commissioners.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        string profession,
        string speciality,
        int qualificationDegreeId,
        DateTime date,
        string commissionNominationOrderNumber,
        DateTime commissionNominationOrderDate,
        int directorPersonId,
        int commissionChairman,
        int[] commissionMembers,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.Profession = profession;
        this.Speciality = speciality;
        this.QualificationDegreeId = qualificationDegreeId;
        this.CommissionNominationOrderNumber = commissionNominationOrderNumber;
        this.CommissionNominationOrderDate = commissionNominationOrderDate;
        this.Date = date;
        this.DirectorPersonId = directorPersonId;

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
        this.commissioners.Add(new QualificationAcquisitionProtocolCommissioner(this, commissionChairman, true, 0));
        this.commissioners.AddRange(commissionMembers.Select((id, index) => new QualificationAcquisitionProtocolCommissioner(this, id, false, index + 1)));
    }

    public QualificationAcquisitionProtocolStudent AddStudent(
        int classId,
        int personId,
        bool examsPassed,
        decimal? theoryPoints,
        decimal? practicePoints,
        decimal? averageDecimalGrade,
        int modifiedBySysUserId)
    {
        var student = new QualificationAcquisitionProtocolStudent(
            this,
            classId,
            personId,
            examsPassed,
            theoryPoints,
            practicePoints,
            averageDecimalGrade);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public void UpdateStudent(
        int classId,
        int personId,
        bool examsPassed,
        decimal? theoryPoints,
        decimal? practicePoints,
        decimal? averageDecimalGrade,
        int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);
        student.UpdateData(
            examsPassed,
            theoryPoints,
            practicePoints,
            averageDecimalGrade);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public QualificationAcquisitionProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);

        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }
}
