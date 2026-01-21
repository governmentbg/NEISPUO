namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveExamDutyProtocolStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ExamDutyProtocolId { get; init; }

    [JsonIgnore] public int? ClassId { get; init; }
    [JsonIgnore] public int? PersonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamDutyProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
