namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSupportExtApiCommand : CreateSupportExtApiCommand
{
    [JsonIgnore]public int? SupportId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.SupportId;
}
