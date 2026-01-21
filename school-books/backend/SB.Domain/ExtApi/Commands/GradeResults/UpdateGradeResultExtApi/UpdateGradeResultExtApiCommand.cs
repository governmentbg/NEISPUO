namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateGradeResultExtApiCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? GradeResultId { get; init; }

    public GradeResultType? InitialResultType { get; init; }
    public CreateUpdateGradeResultExtApiCommandCurriculum[]? RetakeExamCurriculums { get; init; }
    public GradeResultType? FinalResultType { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GradeResult);
    [JsonIgnore]public virtual int? ObjectId => this.GradeResultId;
}
