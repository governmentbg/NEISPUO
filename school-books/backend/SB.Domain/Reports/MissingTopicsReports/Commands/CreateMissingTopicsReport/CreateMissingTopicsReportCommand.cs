namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateMissingTopicsReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public MissingTopicsReportPeriod? Period { get; set; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public int? TeacherPersonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(MissingTopicsReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}
