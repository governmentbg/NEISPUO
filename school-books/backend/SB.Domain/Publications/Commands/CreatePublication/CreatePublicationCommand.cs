namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreatePublicationCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public PublicationType? Type { get; init; }
    public DateTime? Date { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; }
    public CreatePublicationCommandFile[]? Files { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Publication);
    [JsonIgnore]public virtual int? ObjectId => null;
}
