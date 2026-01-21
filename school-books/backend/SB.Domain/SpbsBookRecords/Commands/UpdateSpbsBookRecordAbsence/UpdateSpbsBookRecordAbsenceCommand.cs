namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSpbsBookRecordAbsenceCommand : CreateSpbsBookRecordAbsenceCommand
{
    [JsonIgnore]public int? OrderNum { get; init; }
}
