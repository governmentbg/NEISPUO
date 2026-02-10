namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveNoteCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? NoteId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Note);
    [JsonIgnore]public int? ObjectId => this.NoteId;
}
