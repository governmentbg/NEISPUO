namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record CreateQualificationAcquisitionProtocolStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? QualificationAcquisitionProtocolId { get; init; }

    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
    public bool? ExamsPassed { get; init; }
    public decimal? TheoryPoints { get; init; }
    public decimal? PracticePoints { get; init; }
    public decimal? AverageDecimalGrade { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationAcquisitionProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
