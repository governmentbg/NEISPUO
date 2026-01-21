namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateGradeChangeExamsAdmProtocolStudentCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? GradeChangeExamsAdmProtocolId { get; init; }

    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
    public CreateGradeChangeExamsAdmProtocolStudentCommandSubject[]? Subjects { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GradeChangeExamsAdmProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
