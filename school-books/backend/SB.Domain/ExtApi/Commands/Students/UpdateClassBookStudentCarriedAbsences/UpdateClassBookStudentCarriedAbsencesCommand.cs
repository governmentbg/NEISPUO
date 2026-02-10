namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateClassBookStudentCarriedAbsencesCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public int? FirstTermExcusedCount { get; init; }
    public int? FirstTermUnexcusedCount { get; init; }
    public int? FirstTermLateCount { get; init; }
    public int? SecondTermExcusedCount { get; init; }
    public int? SecondTermUnexcusedCount { get; init; }
    public int? SecondTermLateCount { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBookStudentCarriedAbsence);
    [JsonIgnore]public virtual int? ObjectId => null;
}
