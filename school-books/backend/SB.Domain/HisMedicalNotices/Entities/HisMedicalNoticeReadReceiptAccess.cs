namespace SB.Domain;

public class HisMedicalNoticeReadReceiptAccess
{
    // EF constructor
    private HisMedicalNoticeReadReceiptAccess()
    {
        this.ReadReceipt = null!;
    }

    public HisMedicalNoticeReadReceiptAccess(
        HisMedicalNoticeReadReceipt readReceipt,
        int schoolYear,
        int instId)
    {
        this.ReadReceipt = readReceipt;
        this.SchoolYear = schoolYear;
        this.InstId = instId;
    }

    public int ExtSystemId { get; private set; }

    public int HisMedicalNoticeId { get; private set; }

    public int SchoolYear { get; private set; }

    public int InstId { get; private set; }

    // relations
    public HisMedicalNoticeReadReceipt ReadReceipt { get; private set; }
}
