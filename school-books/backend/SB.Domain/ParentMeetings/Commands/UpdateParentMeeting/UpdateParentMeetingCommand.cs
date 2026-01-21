namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateParentMeetingCommand : CreateParentMeetingCommand
{
    [JsonIgnore]public int? ParentMeetingId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.ParentMeetingId;
}
