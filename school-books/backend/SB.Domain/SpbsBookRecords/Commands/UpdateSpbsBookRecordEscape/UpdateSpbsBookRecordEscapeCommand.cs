namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSpbsBookRecordEscapeCommand : CreateSpbsBookRecordEscapeCommand
{
    [JsonIgnore]public int? OrderNum { get; init; }
}
