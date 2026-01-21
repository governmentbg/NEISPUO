namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class HighSchoolCertificateProtocol : IAggregateRoot
{
    // EF constructor
    private HighSchoolCertificateProtocol()
    {
        this.ProtocolNum = null!;
        this.CommissionNominationOrderNumber = null!;
        this.ExamSession = null!;
        this.Version = null!;
    }

    public HighSchoolCertificateProtocol(
        int schoolYear,
        int instId,
        HighSchoolCertificateProtocolStage stage,
        string? protocolNum,
        DateTime? protocolDate,
        DateTime commissionMeetingDate,
        string commissionNominationOrderNum,
        DateTime commissionNominationOrderDate,
        string? examSession,
        int directorPersonId,
        int commissionChairman,
        int[] commissionMembers,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Stage = stage;
        this.ProtocolNum = protocolNum;
        this.ProtocolDate = protocolDate;
        this.CommissionMeetingDate = commissionMeetingDate;
        this.CommissionNominationOrderNumber = commissionNominationOrderNum;
        this.CommissionNominationOrderDate = commissionNominationOrderDate;
        this.ExamSession = examSession;
        this.DirectorPersonId = directorPersonId;
        this.SetCommissioners(commissionChairman, commissionMembers);

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int HighSchoolCertificateProtocolId { get; private set; }
    public int InstId { get; private set; }
    public HighSchoolCertificateProtocolStage Stage { get; private set; }
    public string? ProtocolNum { get; private set; }
    public DateTime? ProtocolDate { get; private set; }
    public DateTime CommissionMeetingDate { get; private set; }
    public string CommissionNominationOrderNumber { get; private set; }
    public DateTime CommissionNominationOrderDate { get; private set; }
    public string? ExamSession { get; private set; }
    public int DirectorPersonId { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    // relations
    private readonly List<HighSchoolCertificateProtocolCommissioner> commissioners = new();
    public IReadOnlyCollection<HighSchoolCertificateProtocolCommissioner> Commissioners => this.commissioners.AsReadOnly();

    private readonly List<HighSchoolCertificateProtocolStudent> students = new();
    public IReadOnlyCollection<HighSchoolCertificateProtocolStudent> Students => this.students.AsReadOnly();

    public void UpdateData(
        HighSchoolCertificateProtocolStage stage,
        string? protocolNum,
        DateTime? protocolDate,
        DateTime commissionMeetingDate,
        string commissionNominationOrderNum,
        DateTime commissionNominationOrderDate,
        string? examSession,
        int directorPersonId,
        int commissionChairman,
        int[] commissionMembers,
        int modifiedBySysUserId)
    {
        this.Stage = stage;
        this.ProtocolNum = protocolNum;
        this.ProtocolDate = protocolDate;
        this.CommissionMeetingDate = commissionMeetingDate;
        this.CommissionNominationOrderNumber = commissionNominationOrderNum;
        this.CommissionNominationOrderDate = commissionNominationOrderDate;
        this.ExamSession = examSession;
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
        this.commissioners.Add(new HighSchoolCertificateProtocolCommissioner(this, commissionChairman, true, 0));
        this.commissioners.AddRange(commissionMembers.Select((id, index) => new HighSchoolCertificateProtocolCommissioner(this, id, false, index + 1)));
    }

    public HighSchoolCertificateProtocolStudent AddStudent(
        int classId,
        int personId,
        int modifiedBySysUserId)
    {
        var student = new HighSchoolCertificateProtocolStudent(
            this,
            classId,
            personId);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public HighSchoolCertificateProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.Students.Single(s => s.ClassId == classId && s.PersonId == personId);
        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }
}
