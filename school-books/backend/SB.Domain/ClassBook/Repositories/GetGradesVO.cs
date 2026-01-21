namespace SB.Domain;

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial interface IClassBookPrintRepository
{
    [Keyless]
    public record GetGradesVO(
        int PersonId,
        int CurriculumId,
        int GradeId,
        GradeCategory Category,
        GradeType Type,
        SchoolTerm Term,
        [property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        DateTime GradeDate,
        int SubjectTypeId);
}
