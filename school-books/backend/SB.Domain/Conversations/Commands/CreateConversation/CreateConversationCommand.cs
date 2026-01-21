namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;
using static SB.Domain.IConversationsQueryRepository;

public record CreateConversationCommand : IRequest<CreatedConversationVO>, IAuditedCommand
{
    [JsonIgnore] public CreateConversationCommandCreator? Creator { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SchoolYear { get; init; }

    public string? Title { get; init; }
    public string? Message { get; init; }
    public bool? IsLocked { get; init; }
    public CreateConversationCommandParticipant[]? Participants { get; init; }

    [JsonIgnore] public string ObjectName => nameof(Conversation);
    [JsonIgnore] public virtual int? ObjectId => null;
}
