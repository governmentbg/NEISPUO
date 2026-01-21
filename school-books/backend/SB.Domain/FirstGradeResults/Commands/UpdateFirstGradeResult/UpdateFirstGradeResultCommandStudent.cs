namespace SB.Domain;

public record UpdateFirstGradeResultCommandStudent
{
    public int? PersonId { get; init; }
    public QualitativeGrade? QualitativeGrade { get; init; }
    public SpecialNeedsGrade? SpecialGrade { get; init; }
}
