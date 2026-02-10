namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateRemarkExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore] public int? CurriculumId { get; init; }
    public int? PersonId { get; init; }
    public RemarkType? Type { get; init; }
    public DateTime? Date { get; init; }
    public string? Description { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Remark);
    [JsonIgnore]public virtual int? ObjectId => null;
}
