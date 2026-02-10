namespace SB.Domain;

using System;

public record ExamsModel(
    BookExamType Type,
    ExamsModelExam[] Exams
);

public record ExamsModelExam(
    string SubjectName,
    string SubjectTypeName,
    DateTime[] FirstTermDates,
    DateTime[] SecondTermDates
);
