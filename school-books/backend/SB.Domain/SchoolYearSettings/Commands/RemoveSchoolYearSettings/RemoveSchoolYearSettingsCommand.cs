namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSchoolYearSettingsCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? SchoolYearSettingsId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SchoolYearSettings);
    [JsonIgnore]public int? ObjectId => this.SchoolYearSettingsId;
}
