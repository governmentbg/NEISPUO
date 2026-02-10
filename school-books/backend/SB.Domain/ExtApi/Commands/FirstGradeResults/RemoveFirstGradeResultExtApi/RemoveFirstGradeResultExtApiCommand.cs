namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveFirstGradeResultExtApiCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? FirstGradeResultId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(FirstGradeResult);
    [JsonIgnore]public virtual int? ObjectId => this.FirstGradeResultId;
}
