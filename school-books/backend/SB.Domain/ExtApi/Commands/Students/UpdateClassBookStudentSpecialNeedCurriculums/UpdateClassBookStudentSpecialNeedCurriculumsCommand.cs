namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateClassBookStudentSpecialNeedCurriculumsCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public int[]? SpecialNeedCurriculumIds { get; init; }
    public bool? HasSpecialNeedFirstGradeResult { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBookStudentSpecialNeeds);
    [JsonIgnore]public virtual int? ObjectId => null;
}
