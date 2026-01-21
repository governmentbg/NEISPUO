namespace SB.Domain;

public record FirstGradeResultsModel(
    FirstGradeResultsModelFirstGradeResult[] FirstGradeResults
);

public record FirstGradeResultsModelFirstGradeResult(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    QualitativeGrade? QualitativeGrade,
    SpecialNeedsGrade? SpecialGrade
);
