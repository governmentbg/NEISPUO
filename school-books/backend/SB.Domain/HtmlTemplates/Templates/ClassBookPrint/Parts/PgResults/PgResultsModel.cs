namespace SB.Domain;

public record PgResultsModel(
    PgResultsModelPgResult[] PgResults
);

public record PgResultsModelPgResult(
    string FullName,
    string? CurriculumName,
    string? StartSchoolYearResult,
    string? EndSchoolYearResult
);
