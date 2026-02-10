namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSchoolYearSettingsCommand : CreateSchoolYearSettingsCommand
{
    [JsonIgnore]public int? SchoolYearSettingsId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.SchoolYearSettingsId;
}
