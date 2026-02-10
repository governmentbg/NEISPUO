namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateClassBookTopicPlanItemsFromExcelImportCommand : IAuditedCreateMultipleCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? CurriculumId { get; init; }
    [JsonIgnore]public int? BlobId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBookTopicPlanItem);
    [JsonIgnore]public int? ObjectId => null;
}
