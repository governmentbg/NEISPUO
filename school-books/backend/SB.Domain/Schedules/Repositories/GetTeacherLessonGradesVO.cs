namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public partial interface IMyHourQueryRepository
{
    [Keyless]
    public record GetTeacherLessonGradesVO(
        int GradeId,
        int PersonId,
        DateTime Date,
        GradeCategory Category,
        [property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade);
}
