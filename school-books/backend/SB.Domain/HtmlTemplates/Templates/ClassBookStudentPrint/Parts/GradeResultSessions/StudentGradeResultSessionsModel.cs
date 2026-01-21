namespace SB.Domain;

public record StudentGradeResultSessionsModel(
    StudentGradeResultSessionsModelSession[] Sessions,
    string? FinalResultText
);

public record StudentGradeResultSessionsModelSession(
    string SubjectName,
    string? Session1ResultText,
    string? Session2ResultText,
    string? Session3ResultText
);
