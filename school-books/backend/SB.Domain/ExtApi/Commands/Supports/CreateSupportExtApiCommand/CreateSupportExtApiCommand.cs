namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateSupportExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public string? Description { get; init; }
    public string? ExpectedResult { get; init; }
    public DateTime? EndDate { get; init; }
    public int? CreatedBySysUserId { get; init; }
    public bool? IsForAllStudents { get; init; }
    public int[]? StudentIds { get; init; }
    public int[]? TeacherIds { get; init; }
    public int[]? SupportDifficultyTypeIds { get; init; }
    public CreateSupportExtApiCommandActivity[]? Activities { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Support);
    [JsonIgnore]public virtual int? ObjectId => null;
}
