namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record UpdateTakenClassBookTopicPlanItemCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? CurriculumId { get; init; }
    [JsonIgnore]public int? ClassBookTopicPlanItemId { get; init; }

    public bool? Taken { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBookTopicPlanItem);
    [JsonIgnore]public int? ObjectId => this.ClassBookTopicPlanItemId;
}
