namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateNvoExamDutyProtocolStudentCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? NvoExamDutyProtocolId { get; init; }

    public CreateNvoExamDutyProtocolStudentCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(NvoExamDutyProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
