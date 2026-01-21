namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateRemarkExtApiCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore] public bool IsExternal { get; init; }
    [JsonIgnore] public int? CurriculumId { get; init; }
    [JsonIgnore] public int? RemarkId { get; init; }
    public DateTime? Date { get; init; }
    public string? Description { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Remark);
    [JsonIgnore]public int? ObjectId => this.RemarkId;
}
