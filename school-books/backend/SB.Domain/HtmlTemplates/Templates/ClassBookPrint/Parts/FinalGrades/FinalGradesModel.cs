namespace SB.Domain;

public record FinalGradesModel(
    FinalGradesModelCurriculumInfo[] Curriculum,
    FinalGradesModelStudentInfo[] Students,
    FinalGradesModelItem[] Items
);

public record FinalGradesModelCurriculumInfo(
    int CurriculumId,
    string SubjectName,
    string SubjectTypeName
);

public record FinalGradesModelStudentInfo(
    int PersonId,
    int? ClassNumber,
    string FullName,
    bool IsTransferred
);

public record FinalGradesModelItem(
    int PersonId,
    FinalGradesModelItemByCurriculum[] Curriculum
);

public record FinalGradesModelItemByCurriculum(
    int CurriculumId,
    FinalGradesModelItemByType GradesByType
);

public record FinalGradesModelItemByType(
    string? FirstTermGrade,
    string? SecondTermGrade,
    string? FinalGrade
);
