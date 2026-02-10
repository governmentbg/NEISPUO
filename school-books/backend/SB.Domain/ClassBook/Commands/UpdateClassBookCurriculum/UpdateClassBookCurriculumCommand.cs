namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateClassBookCurriculumCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? CurriculumId { get; init; }
    public bool? WithoutGrade { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBook);
    [JsonIgnore]public virtual int? ObjectId => null;
}
