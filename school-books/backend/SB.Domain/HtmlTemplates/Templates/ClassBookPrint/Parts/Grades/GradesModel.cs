namespace SB.Domain;

public record GradesModel(
    SchoolTerm Term,
    GradesModelCurriculumInfo[] Curriculum,
    GradesModelStudentInfo[] Students,
    GradesModelItem[] Items
);

public record GradesModelCurriculumInfo(
    int CurriculumId,
    string SubjectName,
    string SubjectTypeName
);

public record GradesModelStudentInfo(
    int PersonId,
    int? ClassNumber,
    string FullName,
    bool IsTransferred
);

public record GradesModelItem(
    int PersonId,
    GradesModelItemByCurriculum[] Curriculum
);

public record GradesModelItemByCurriculum(
    int CurriculumId,
    GradesModelItemByMonths GradesByMonths
);

public record GradesModelItemByMonths(
    string[] SeptemberGrades,
    string[] OctoberGrades,
    string[] NovemberGrades,
    string[] DecemberGrades,
    string[] JanuaryGrades,
    string[] FebruaryGrades,
    string[] MarchGrades,
    string[] AprilGrades,
    string[] MayGrades,
    string[] JuneGrades,
    string[] TermGrade
);
