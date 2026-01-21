namespace SB.Domain;

public record GradeResultSessionsModel(
    GradeResultSessionsModelStudent[] Students
);

public record GradeResultSessionsModelStudent(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    GradeResultSessionsModelSession[] Sessions,
    string? FinalResultText
);

public record GradeResultSessionsModelSession(
    string SubjectName,
    string? Session1ResultText,
    string? Session2ResultText,
    string? Session3ResultText
);
