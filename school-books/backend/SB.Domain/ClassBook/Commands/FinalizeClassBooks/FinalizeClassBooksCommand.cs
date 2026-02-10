namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record FinalizeClassBooksCommand : IRequest, IAuditedUpdateMultipleCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int[]? ClassBookIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBook);
    [JsonIgnore]public virtual int? ObjectId => null;
    [JsonIgnore]public int[] ObjectIds => this.ClassBookIds ?? Array.Empty<int>();
}
