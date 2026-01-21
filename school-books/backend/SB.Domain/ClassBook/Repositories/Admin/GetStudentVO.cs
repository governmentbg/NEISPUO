namespace SB.Domain;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetStudentVO(
        int PersonId,
        int? ClassNumber,
        string StudentFullName,
        int[] SpecialNeedsCurriculumIds,
        bool HasSpecialNeedFirstGradeResult,
        GetStudentVOGradelessCurriculum[] GradelessCurriculumIds,
        GetStudentVOCurriculums[] StudentCurriculums,
        string? Activities,
        string? Speciality,
        GetStudentVOCarriedAbsences? CarriedAbsences);

    public record GetStudentVOGradelessCurriculum(
        int CurriculumId,
        bool WithoutFirstTermGrade,
        bool WithoutSecondTermGrade,
        bool WithoutFinalGrade);

    public record GetStudentVOCurriculums(
        int CurriculumId,
        string CurriculumName,
        string? CurriculumGroupName);

    public record GetStudentVOCarriedAbsences(
        int FirstTermExcusedCount,
        int FirstTermUnexcusedCount,
        int FirstTermLateCount,
        int SecondTermExcusedCount,
        int SecondTermUnexcusedCount,
        int SecondTermLateCount);
}
