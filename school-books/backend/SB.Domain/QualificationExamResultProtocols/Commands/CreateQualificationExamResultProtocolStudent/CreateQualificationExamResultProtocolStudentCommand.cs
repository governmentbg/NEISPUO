namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record CreateQualificationExamResultProtocolStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? QualificationExamResultProtocolId { get; init; }

    public CreateQualificationExamResultProtocolStudentCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationExamResultProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
