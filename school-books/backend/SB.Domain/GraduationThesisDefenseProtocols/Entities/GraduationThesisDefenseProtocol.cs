namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class GraduationThesisDefenseProtocol : IAggregateRoot
{
    // EF constructor
    private GraduationThesisDefenseProtocol()
    {
        this.Version = null!;
        this.DirectorOrderNumber = null!;
    }

    public GraduationThesisDefenseProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        int? eduFormId,
        DateTime commissionMeetingDate,
        string directorOrderNumber,
        DateTime directorOrderDate,
        int directorPersonId,
        int commissionChairman,
        int[] commissionMembers,
        int section1StudentsCapacity,
        int section2StudentsCapacity,
        int section3StudentsCapacity,
        int section4StudentsCapacity,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.EduFormId = eduFormId;
        this.CommissionMeetingDate = commissionMeetingDate;
        this.DirectorOrderNumber = directorOrderNumber;
        this.DirectorOrderDate = directorOrderDate;
        this.DirectorPersonId = directorPersonId;
        this.Section1StudentsCapacity = section1StudentsCapacity;
        this.Section2StudentsCapacity = section2StudentsCapacity;
        this.Section3StudentsCapacity = section3StudentsCapacity;
        this.Section4StudentsCapacity = section4StudentsCapacity;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.SetCommissioners(commissionChairman, commissionMembers);
    }


    public int SchoolYear { get; private set; }

    public int GraduationThesisDefenseProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public DateTime? ProtocolDate { get; private set; }

    public string? SessionType { get; private set; }

    public int? EduFormId { get; private set; }

    public DateTime CommissionMeetingDate { get; private set; }

    public string DirectorOrderNumber { get; private set; }

    public DateTime DirectorOrderDate { get; private set; }

    public int DirectorPersonId { get; private set; }

    public int Section1StudentsCapacity { get; private set; }

    public int Section2StudentsCapacity { get; private set; }

    public int Section3StudentsCapacity { get; private set; }

    public int Section4StudentsCapacity { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<GraduationThesisDefenseProtocolCommissioner> commissioners = new();
    public IReadOnlyCollection<GraduationThesisDefenseProtocolCommissioner> Commissioners => this.commissioners.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        DateTime? protocolDate,
        string? sessionType,
        int? eduFormId,
        DateTime commissionMeetingDate,
        string directorOrderNumber,
        DateTime directorOrderDate,
        int directorPersonId,
        int commissionChairman,
        int[] commissionMembers,
        int section1StudentsCapacity,
        int section2StudentsCapacity,
        int section3StudentsCapacity,
        int section4StudentsCapacity,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.ProtocolDate = protocolDate;
        this.SessionType = sessionType;
        this.EduFormId = eduFormId;
        this.CommissionMeetingDate = commissionMeetingDate;
        this.DirectorOrderNumber = directorOrderNumber;
        this.DirectorOrderDate = directorOrderDate;
        this.DirectorPersonId = directorPersonId;
        this.Section1StudentsCapacity = section1StudentsCapacity;
        this.Section2StudentsCapacity = section2StudentsCapacity;
        this.Section3StudentsCapacity = section3StudentsCapacity;
        this.Section4StudentsCapacity = section4StudentsCapacity;

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
        this.commissioners.Add(new GraduationThesisDefenseProtocolCommissioner(this, commissionChairman, true, 0));
        this.commissioners.AddRange(commissionMembers.Select((id, index) => new GraduationThesisDefenseProtocolCommissioner(this, id, false, index + 1)));
    }
}
