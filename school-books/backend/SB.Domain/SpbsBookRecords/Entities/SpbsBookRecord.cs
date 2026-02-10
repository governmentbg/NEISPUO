namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class SpbsBookRecord : IAggregateRoot
{
    // EF constructor
    private SpbsBookRecord()
    {
        this.Version = null!;
    }

    public SpbsBookRecord(
        int schoolYear,
        int instId,
        int recordNumber,
        int classId,
        int personId,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.RecordNumber = recordNumber;
        this.ClassId = classId;
        this.PersonId = personId;

        this.movements.Add(new SpbsBookRecordMovement(this, 0));

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int SpbsBookRecordId { get; private set; }

    public int InstId { get; private set; }

    public int RecordNumber { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }

    public string? SendingCommission { get; private set; }

    public string? SendingCommissionAddress { get; private set; }

    public string? SendingCommissionPhoneNumber { get; private set; }

    public string? InspectorNames { get; private set; }

    public string? InspectorAddress { get; private set; }

    public string? InspectorPhoneNumber { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    private readonly List<SpbsBookRecordMovement> movements = new();
    public IReadOnlyCollection<SpbsBookRecordMovement> Movements => this.movements.AsReadOnly();

    private readonly List<SpbsBookRecordEscape> escapes = new();
    public IReadOnlyCollection<SpbsBookRecordEscape> Escapes => this.escapes.AsReadOnly();

    private readonly List<SpbsBookRecordAbsence> absences = new();
    public IReadOnlyCollection<SpbsBookRecordAbsence> Absences => this.absences.AsReadOnly();

    public void Update(
        string? sendingCommission,
        string? sendingCommissionAddress,
        string? sendingCommissionPhoneNumber,
        string? inspectorNames,
        string? inspectorAddress,
        string? inspectorPhoneNumber,
        string? courtDecisionNumber,
        DateTime? courtDecisionDate,
        int? incomingInstId,
        string? incommingLetterNumber,
        DateTime? incommingLetterDate,
        DateTime? incommingDate,
        string? incommingDocNumber,
        int? transferInstId,
        string? transferReason,
        string? transferProtocolNumber,
        DateTime? transferProtocolDate,
        string? transferLetterNumber,
        DateTime? transferLetterDate,
        string? transferCertificateNumber,
        DateTime? transferCertificateDate,
        string? transferMessageNumber,
        DateTime? transferMessageDate,
        int modifiedBySysUserId)
    {
        this.SendingCommission = sendingCommission;
        this.SendingCommissionAddress = sendingCommissionAddress;
        this.SendingCommissionPhoneNumber = sendingCommissionPhoneNumber;
        this.InspectorNames = inspectorNames;
        this.InspectorAddress = inspectorAddress;
        this.InspectorPhoneNumber = inspectorPhoneNumber;

        this.movements[0].Update(
            courtDecisionNumber,
            courtDecisionDate,
            incomingInstId,
            incommingLetterNumber,
            incommingLetterDate,
            incommingDate,
            incommingDocNumber,
            transferInstId,
            transferReason,
            transferProtocolNumber,
            transferProtocolDate,
            transferLetterNumber,
            transferLetterDate,
            transferCertificateNumber,
            transferCertificateDate,
            transferMessageNumber,
            transferMessageDate);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SpbsBookRecordEscape AddEscape(
        DateTime escapeDate,
        TimeSpan escapeTime,
        DateTime policeNotificationDate,
        TimeSpan policeNotificationTime,
        string policeLetterNumber,
        DateTime policeLetterDate,
        DateTime? returnDate,
        int modifiedBySysUserId)
    {
        var escape = new SpbsBookRecordEscape(
            this,
            this.escapes.Count,
            escapeDate,
            escapeTime,
            policeNotificationDate,
            policeNotificationTime,
            policeLetterNumber,
            policeLetterDate,
            returnDate);

        this.escapes.Add(escape);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return escape;
    }

    public void UpdateEscape(
        int escapeOrderNum,
        DateTime escapeDate,
        TimeSpan escapeTime,
        DateTime policeNotificationDate,
        TimeSpan policeNotificationTime,
        string policeLetterNumber,
        DateTime policeLetterDate,
        DateTime? returnDate,
        int modifiedBySysUserId)
    {
        var escape = this.escapes.Single(e => e.OrderNum == escapeOrderNum);
        escape.Update(
            escapeDate,
            escapeTime,
            policeNotificationDate,
            policeNotificationTime,
            policeLetterNumber,
            policeLetterDate,
            returnDate);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SpbsBookRecordEscape RemoveEscape(int ordredNum, int modifiedBySysUserId)
    {
        var escape = this.Escapes.Single(i => i.OrderNum == ordredNum);
        this.escapes.Remove(escape);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return escape;
    }

    public SpbsBookRecordAbsence AddAbsence(
        DateTime absenceDate,
        string absenceReason,
        int modifiedBySysUserId)
    {
        var absence = new SpbsBookRecordAbsence(
            this,
            this.absences.Count,
            absenceDate,
            absenceReason);

        this.absences.Add(absence);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return absence;
    }

    public void UpdateAbsence(
        int absenceOrderNum,
        DateTime absenceDate,
        string absenceReason,
        int modifiedBySysUserId)
    {
        var absence = this.absences.Single(a => a.OrderNum == absenceOrderNum);
        absence.Update(
            absenceDate,
            absenceReason);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SpbsBookRecordAbsence RemoveAbsence(int ordredNum, int modifiedBySysUserId)
    {
        var absence = this.Absences.Single(i => i.OrderNum == ordredNum);
        this.absences.Remove(absence);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return absence;
    }
}
