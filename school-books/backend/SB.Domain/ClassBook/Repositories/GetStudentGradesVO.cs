namespace SB.Domain;

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial interface IClassBookStudentPrintRepository
{
    [Keyless]
    public record GetStudentGradesVO(
        int CurriculumId,
        int? ParentCurriculumId,
        int SubjectId,
        string CurriculumName,
        int SubjectTypeId,
        int? CurriculumPartID,
        int? IsIndividualLesson,
        int? TotalTermHours,
        bool CurriculumIsValid,
        GradeCategory Category,
        GradeType Type,
        SchoolTerm Term,
        decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        DateTime GradeDate)
    {
        [Column(TypeName = "DECIMAL(3,2)")]
        public decimal? DecimalGrade { get; init; } = DecimalGrade;
    }
}
