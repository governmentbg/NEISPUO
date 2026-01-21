namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public partial interface IStudentInfoClassBooksQueryRepository
{
    [Keyless]
    public record GetStudentInfoGradeVO(
        int GradeId,
        int SubjectId,
        string SubjectName,
        string? SubjectNameShort,
        int SubjectTypeId,
        string SubjectTypeName,
        string BasicSubjectTypeNameShort,
        bool BasicSubjectTypeIsMandatoryCurriculum,
        DateTime Date,
        GradeType Type,
        SchoolTerm Term,
        GradeCategory Category,
        [property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        string? Comment);
}
