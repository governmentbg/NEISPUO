namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateClassBookStudentActivitiesCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public string? Activities { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StudentClass);
    [JsonIgnore]public virtual int? ObjectId => null;
}
