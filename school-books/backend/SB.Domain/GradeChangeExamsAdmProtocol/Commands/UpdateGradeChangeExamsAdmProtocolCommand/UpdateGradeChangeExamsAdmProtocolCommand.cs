namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateGradeChangeExamsAdmProtocolCommand : CreateGradeChangeExamsAdmProtocolCommand
{
    [JsonIgnore]public int? GradeChangeExamsAdmProtocolId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.GradeChangeExamsAdmProtocolId;
}
