namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class HisMedicalNoticeReadReceipt : IAggregateRoot
{
    // EF constructor
    private HisMedicalNoticeReadReceipt()
    {
    }

    public HisMedicalNoticeReadReceipt(int extSystemId, int hisMedicalNoticeId)
        : this(extSystemId, hisMedicalNoticeId, Array.Empty<(int schoolYear, int instId)>())
    {
    }

    public HisMedicalNoticeReadReceipt(int extSystemId, int hisMedicalNoticeId, (int schoolYear, int instId)[] accesses)
    {
        this.ExtSystemId = extSystemId;
        this.HisMedicalNoticeId = hisMedicalNoticeId;
        this.IsAcknowledged = false;

        this.accesses.AddRange(
            accesses.Select(access =>
                new HisMedicalNoticeReadReceiptAccess(
                    this,
                    access.schoolYear,
                    access.instId)));

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
    }

    public int ExtSystemId { get; private set; }

    public int HisMedicalNoticeId { get; private set; }

    public bool IsAcknowledged { get; private set; }

    public DateTime CreateDate { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public byte[] Version { get; private set; } = null!;

    private readonly List<HisMedicalNoticeReadReceiptAccess> accesses = new();
    public IReadOnlyCollection<HisMedicalNoticeReadReceiptAccess> Accesses => this.accesses.AsReadOnly();

    public void SetAcknowledged()
    {
        this.IsAcknowledged = true;
        this.ModifyDate = DateTime.Now;
    }
}
