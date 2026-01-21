namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateExamCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore] public int? CurriculumId { get; init; }
    public BookExamType? Type { get; init; }
    public DateTime? Date { get; init; }
    public string? Description { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Exam);
    [JsonIgnore]public virtual int? ObjectId => null;
}
