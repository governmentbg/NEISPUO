namespace SB.Domain;

public record StudentFirstGradeResultsModel(
    QualitativeGrade? QualitativeGrade,
    SpecialNeedsGrade? SpecialGrade
);
