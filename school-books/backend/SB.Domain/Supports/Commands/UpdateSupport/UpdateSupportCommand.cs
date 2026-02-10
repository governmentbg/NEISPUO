namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSupportCommand : CreateSupportCommand
{
    [JsonIgnore]public int? SupportId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.SupportId;
}
