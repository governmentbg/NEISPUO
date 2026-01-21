namespace SB.Domain;

using System;

public record StudentExamsModel(
    BookExamType Type,
    StudentExamsModelExam[] Exams
);

public record StudentExamsModelExam(
    string SubjectName,
    string SubjectTypeName,
    DateTime[] FirstTermDates,
    DateTime[] SecondTermDates
);
