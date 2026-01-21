namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdatePublicationCommand : CreatePublicationCommand
{
    [JsonIgnore]public int? PublicationId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.PublicationId;
}
