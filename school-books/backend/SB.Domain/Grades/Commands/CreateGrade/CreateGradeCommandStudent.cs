namespace SB.Domain;

public record CreateGradeCommandStudent
{
    public int? PersonId { get; init; }
    public decimal? DecimalGrade { get; init; }
    public QualitativeGrade? QualitativeGrade { get; init; }
    public SpecialNeedsGrade? SpecialGrade { get; init; }
    public string? Comment { get; init; }
}
