namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record UpdateNoteCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? NoteId { get; init; }


    public string? Description { get; init; }
    public bool? IsForAllStudents { get; init; }
    public int[]? StudentIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Note);
    [JsonIgnore]public int? ObjectId => this.NoteId;
}
