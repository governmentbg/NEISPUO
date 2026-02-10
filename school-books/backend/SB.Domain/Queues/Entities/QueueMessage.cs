namespace SB.Domain;

using System;

public class QueueMessage
{
    // EF constructor
    private QueueMessage()
    {
        this.Payload = null!;
        this.Version = null!;
    }

    public QueueMessageType Type { get; private set; }

    public long DueDateUnixTimeMs { get; private set; }

    public int QueueMessageId { get; private set; }

    public QueueMessageStatus Status { get; private set; }

    public string? Key { get; private set; }

    public string? Tag { get; private set; }

    public string Payload { get; private set; }

    public int FailedAttempts { get; private set; }

    public string? FailedAttemptsErrors { get; private set; }

    public DateTime CreateDateUtc { get; private set; }

    public DateTime StatusDateUtc { get; private set; }

    public byte[] Version { get; private set; }
}
