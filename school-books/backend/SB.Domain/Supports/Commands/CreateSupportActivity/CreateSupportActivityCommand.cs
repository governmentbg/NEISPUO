namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record CreateSupportActivityCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? SupportId { get; init; }

    public int? SupportActivityTypeId { get; init; }
    public DateTime? Date { get; init; }
    public string? Target { get; init; }
    public string? Result { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SupportActivity);
    [JsonIgnore]public virtual int? ObjectId => null;
}
