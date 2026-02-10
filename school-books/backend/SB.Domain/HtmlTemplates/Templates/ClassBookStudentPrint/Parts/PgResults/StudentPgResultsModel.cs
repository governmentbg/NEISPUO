namespace SB.Domain;

public record StudentPgResultsModel(
    StudentPgResultsModelPgResult[] PgResults
);

public record StudentPgResultsModelPgResult(
    string? StartSchoolYearResult,
    string? EndSchoolYearResult
);
