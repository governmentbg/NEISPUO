namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateNoteCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public string? Description { get; init; }
    public bool? IsForAllStudents { get; init; }
    public int[]? StudentIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Note);
    [JsonIgnore]public virtual int? ObjectId => null;
}
