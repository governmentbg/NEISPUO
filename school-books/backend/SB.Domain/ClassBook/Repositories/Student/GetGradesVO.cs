namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetGradesVO(
        int CurriculumId,
        string CurriculumName,
        GetGradesVOGrade[] FirstTermRegularGrades,
        GetGradesVOGrade[] FirstTermTermGrades,
        GetGradesVOGrade[] SecondTermRegularGrades,
        GetGradesVOGrade[] SecondTermTermGrades,
        GetGradesVOGrade[] FinalGrades);

    public record GetGradesVOGrade(
        int CurriculumId,
        int GradeId,
        GradeCategory Category,
        GradeType Type,
        SchoolTerm Term,
        decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        DateTime GradeDate,
        bool IsReadFromParent);
}
