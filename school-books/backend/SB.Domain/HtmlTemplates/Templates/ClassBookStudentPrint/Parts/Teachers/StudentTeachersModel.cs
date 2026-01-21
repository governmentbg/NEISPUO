namespace SB.Domain;

public record StudentTeachersModel(
    StudentTeachersModelSubject[] Subjects
);

public record StudentTeachersModelSubject(
    string SubjectName,
    string SubjectTypeName,
    string? TeacherName
);
