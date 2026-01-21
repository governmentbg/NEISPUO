namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateClassBookStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public int? ClassNumber { get; init; }
    public int[]? SpecialNeedCurriculumIds { get; init; }
    public bool? HasSpecialNeedFirstGradeResult { get; init; }
    public UpdateClassBookStudentCommandGradeless[]? GradelessCurriculums { get; init; }
    public string? Activities { get; init; }
    public UpdateClassBookStudentCommandCarriedAbsences? CarriedAbsences { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBook);
    [JsonIgnore]public virtual int? ObjectId => null;
}
