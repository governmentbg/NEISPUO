namespace SB.Domain;

using System.Text.Json.Serialization;

public record AddConversationMessageCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? ConversationId { get; init; }
    public string? Message { get; init; }

    [JsonIgnore] public string ObjectName => nameof(ConversationMessage);
    [JsonIgnore] public virtual int? ObjectId => null;
}
