namespace SB.Domain;

using System;

public class HisMedicalNoticeBatch : IAggregateRoot
{
    // EF constructor
    private HisMedicalNoticeBatch()
    {
        this.RequestId = null!;
    }

    public HisMedicalNoticeBatch(string requestId, string? error)
    {
        this.CreateDate = DateTime.Now;
        this.RequestId = requestId;
        this.Error = error;
    }

    public int HisMedicalNoticeBatchId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public string RequestId { get; private set; }

    public string? Error { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public void SetError(string? error)
    {
        this.Error = error;
    }
}
