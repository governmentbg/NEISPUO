namespace SB.Domain;

using System;

public class SpbsBookRecordMovement
{
    // EF constructor
    private SpbsBookRecordMovement()
    {
        this.SpbsBookRecord = null!;
    }

    internal SpbsBookRecordMovement(
        SpbsBookRecord spbsBookRecord,
        int orderNum)
    {
        this.SpbsBookRecord = spbsBookRecord;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }

    public int SpbsBookRecordId { get; private set; }

    public int OrderNum { get; private set; }

    public string? CourtDecisionNumber { get; private set; }

    public DateTime? CourtDecisionDate { get; private set; }

    public int? IncomingInstId { get; private set; }

    public string? IncommingLetterNumber { get; private set; }

    public DateTime? IncommingLetterDate { get; private set; }

    public DateTime? IncommingDate { get; private set; }

    public string? IncommingDocNumber { get; private set; }

    public int? TransferInstId { get; private set; }

    public string? TransferReason { get; private set; }

    public string? TransferProtocolNumber { get; private set; }

    public DateTime? TransferProtocolDate { get; private set; }

    public string? TransferLetterNumber { get; private set; }

    public DateTime? TransferLetterDate { get; private set; }

    public string? TransferCertificateNumber { get; private set; }

    public DateTime? TransferCertificateDate { get; private set; }

    public string? TransferMessageNumber { get; private set; }

    public DateTime? TransferMessageDate { get; private set; }

    // relations
    public SpbsBookRecord SpbsBookRecord { get; private set; }

    internal void Update(
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
        DateTime? transferMessageDate)
    {
        this.CourtDecisionNumber = courtDecisionNumber;
        this.CourtDecisionDate = courtDecisionDate;
        this.IncomingInstId = incomingInstId;
        this.IncommingLetterNumber = incommingLetterNumber;
        this.IncommingLetterDate = incommingLetterDate;
        this.IncommingDate = incommingDate;
        this.IncommingDocNumber = incommingDocNumber;
        this.TransferInstId = transferInstId;
        this.TransferReason = transferReason;
        this.TransferProtocolNumber = transferProtocolNumber;
        this.TransferProtocolDate = transferProtocolDate;
        this.TransferLetterNumber = transferLetterNumber;
        this.TransferLetterDate = transferLetterDate;
        this.TransferCertificateNumber = transferCertificateNumber;
        this.TransferCertificateDate = transferCertificateDate;
        this.TransferMessageNumber = transferMessageNumber;
        this.TransferMessageDate = transferMessageDate;
    }
}
