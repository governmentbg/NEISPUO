namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record SortStudentClassNumbersCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBook);
    [JsonIgnore] public int? ObjectId => this.ClassBookId;
}
