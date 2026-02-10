namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record CreateStateExamsAdmProtocolStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? StateExamsAdmProtocolId { get; init; }

    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
    public bool? HasFirstMandatorySubject { get; init; }
    public CreateStateExamsAdmProtocolStudentCommandSubject? SecondMandatorySubject { get; init; }
    public CreateStateExamsAdmProtocolStudentCommandSubject[]? AdditionalSubjects { get; init; }
    public CreateStateExamsAdmProtocolStudentCommandSubject[]? QualificationSubjects { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StateExamsAdmProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
