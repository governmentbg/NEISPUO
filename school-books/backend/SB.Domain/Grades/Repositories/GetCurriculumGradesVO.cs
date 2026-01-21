namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public partial interface IGradesQueryRepository
{
    [Keyless]
    public record GetCurriculumGradesVO(
        int GradeId,
        int PersonId,
        DateTime Date,
        GradeType Type,
        SchoolTerm Term,
        GradeCategory Category,
        [property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        DateTime CreateDate,
        int CreatedBySysUserId);
}
