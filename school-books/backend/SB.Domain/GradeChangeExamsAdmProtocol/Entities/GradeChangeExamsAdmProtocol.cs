namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class GradeChangeExamsAdmProtocol : IAggregateRoot
{
    // EF constructor
    private GradeChangeExamsAdmProtocol()
    {
        this.CommissionNominationOrderNumber = null!;
        this.Version = null!;
    }

    public GradeChangeExamsAdmProtocol(
        int schoolYear,
        int instId,
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
    public int GradeChangeExamsAdmProtocolId { get; private set; }
    public int InstId { get; private set; }
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
    private readonly List<GradeChangeExamsAdmProtocolCommissioner> commissioners = new();
    public IReadOnlyCollection<GradeChangeExamsAdmProtocolCommissioner> Commissioners => this.commissioners.AsReadOnly();

    private readonly List<GradeChangeExamsAdmProtocolStudent> students = new();
    public IReadOnlyCollection<GradeChangeExamsAdmProtocolStudent> Students => this.students.AsReadOnly();

    public void UpdateData(
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
        this.commissioners.Add(new GradeChangeExamsAdmProtocolCommissioner(this, commissionChairman, true, 0));
        this.commissioners.AddRange(commissionMembers.Select((id, index) => new GradeChangeExamsAdmProtocolCommissioner(this, id, false, index + 1)));
    }

    public GradeChangeExamsAdmProtocolStudent AddStudent(
        int classId,
        int personId,
        (int subjectId, int subjectTypeId)[] subjects,
        int modifiedBySysUserId)
    {
        var student = new GradeChangeExamsAdmProtocolStudent(
            this,
            classId,
            personId,
            subjects);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public void UpdateStudent(
        int classId,
        int personId,
        (int subjectId, int subjectTypeId)[] subjects,
        int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);
        student.UpdateData(subjects);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public GradeChangeExamsAdmProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.Students.Single(s => s.ClassId == classId && s.PersonId == personId);
        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }
}
