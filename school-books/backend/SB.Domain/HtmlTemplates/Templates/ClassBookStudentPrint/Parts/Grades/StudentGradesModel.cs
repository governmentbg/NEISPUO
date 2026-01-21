namespace SB.Domain;

using System;

public record StudentGradesModel(
    SchoolTerm Term,
    StudentGradesModelGrade[] Grades
);

public record StudentGradesModelGrade(
    DateTime Date,
    string CurriculumName,
    string GradeText
);
