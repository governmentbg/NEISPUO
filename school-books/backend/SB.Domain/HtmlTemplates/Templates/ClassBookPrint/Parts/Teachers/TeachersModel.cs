namespace SB.Domain;

public record TeachersModel(
    TeachersModelSubject[] Subjects
);

public record TeachersModelSubject(
    string SubjectName,
    string SubjectTypeName,
    string? TeacherName
);
