namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateExamResultProtocolStudentCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ExamResultProtocolId { get; init; }

    public CreateExamResultProtocolStudentCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamResultProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
