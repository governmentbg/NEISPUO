namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateExamDutyProtocolStudentCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ExamDutyProtocolId { get; init; }

    public CreateExamDutyProtocolStudentCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamDutyProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
