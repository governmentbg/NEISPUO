namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class StateExamsAdmProtocol : IAggregateRoot
{
    // EF constructor
    private StateExamsAdmProtocol()
    {
        this.CommissionNominationOrderNumber = null!;
        this.Version = null!;
    }

    public StateExamsAdmProtocol(
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
    public int StateExamsAdmProtocolId { get; private set; }
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
    private readonly List<StateExamsAdmProtocolCommissioner> commissioners = new();
    public IReadOnlyCollection<StateExamsAdmProtocolCommissioner> Commissioners => this.commissioners.AsReadOnly();

    private readonly List<StateExamsAdmProtocolStudent> students = new();
    public IReadOnlyCollection<StateExamsAdmProtocolStudent> Students => this.students.AsReadOnly();

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
        this.commissioners.Add(new StateExamsAdmProtocolCommissioner(this, commissionChairman, true, 0));
        this.commissioners.AddRange(commissionMembers.Select((id, index) => new StateExamsAdmProtocolCommissioner(this, id, false, index + 1)));
    }

    public StateExamsAdmProtocolStudent AddStudent(
        int classId,
        int personId,
        bool hasFirstMandatorySubject,
        int? secondMandatorySubjectId,
        int? secondMandatorySubjectTypeId,
        (int subjectId, int subjectTypeId)[] additionalSubjects,
        (int subjectId, int subjectTypeId)[] qualificationSubjects,
        int modifiedBySysUserId)
    {
        var student = new StateExamsAdmProtocolStudent(
            this,
            classId,
            personId,
            hasFirstMandatorySubject,
            secondMandatorySubjectId,
            secondMandatorySubjectTypeId,
            additionalSubjects,
            qualificationSubjects);

        this.students.Add(student);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }

    public void UpdateStudent(
        int classId,
        int personId,
        bool hasFirstMandatorySubject,
        int? secondMandatorySubjectId,
        int? secondMandatorySubjectTypeId,
        (int subjectId, int subjectTypeId)[] additionalSubjects,
        (int subjectId, int subjectTypeId)[] qualificationSubjects,
        int modifiedBySysUserId)
    {
        var student = this.students.Single(s => s.ClassId == classId && s.PersonId == personId);
        student.UpdateData(
            hasFirstMandatorySubject,
            secondMandatorySubjectId,
            secondMandatorySubjectTypeId,
            additionalSubjects,
            qualificationSubjects);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public StateExamsAdmProtocolStudent RemoveStudent(int classId, int personId, int modifiedBySysUserId)
    {
        var student = this.Students.Single(s => s.ClassId == classId && s.PersonId == personId);
        this.students.Remove(student);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return student;
    }
}
