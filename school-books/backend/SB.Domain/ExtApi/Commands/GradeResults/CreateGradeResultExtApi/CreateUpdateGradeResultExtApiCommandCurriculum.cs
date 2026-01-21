namespace SB.Domain;

public record CreateUpdateGradeResultExtApiCommandCurriculum
{
    public int? CurriculumId { get; init; }
    public int? Session1Grade { get; init; }
    public bool? Session1NoShow { get; init; }
    public int? Session2Grade { get; init; }
    public bool? Session2NoShow { get; init; }
    public int? Session3Grade { get; init; }
    public bool? Session3NoShow { get; init; }
}
