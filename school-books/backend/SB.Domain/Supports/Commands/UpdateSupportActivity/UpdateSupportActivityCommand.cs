namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSupportActivityCommand : CreateSupportActivityCommand
{
    [JsonIgnore]public int? SupportActivityId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.SupportActivityId;
}
