namespace SB.Domain;

using Data;

public record CreateConversationCommandParticipant
{
    public int? InstId { get; set; }

    public int? SysUserId { get; init; }

    public string? Title { get; init; }

    public int? ClassBookId { get; init; }

    public ParticipantType? ParticipantType { get; init; }
}
