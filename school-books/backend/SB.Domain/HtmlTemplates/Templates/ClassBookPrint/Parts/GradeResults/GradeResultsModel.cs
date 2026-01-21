namespace SB.Domain;

public record GradeResultsModel(
    GradeResultsModelResult[] Results
);

public record GradeResultsModelResult(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    string? ResultText
);
