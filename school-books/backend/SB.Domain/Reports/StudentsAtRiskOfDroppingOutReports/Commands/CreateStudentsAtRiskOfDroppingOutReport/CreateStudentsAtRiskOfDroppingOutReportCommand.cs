namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateStudentsAtRiskOfDroppingOutReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public DateTime? ReportDate { get; set; }

    [JsonIgnore] public string ObjectName => nameof(StudentsAtRiskOfDroppingOutReport);
    [JsonIgnore] public virtual int? ObjectId => null;
}
