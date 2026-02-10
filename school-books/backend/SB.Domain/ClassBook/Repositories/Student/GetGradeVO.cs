namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

public partial interface IStudentClassBooksQueryRepository
{
    [Keyless]
    public record GetGradeVO(
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
        [property: JsonIgnore][property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        string? Comment)
    {
        [NotMapped]
        public string DecimalGradeText => GradeUtils.GetDecimalGradeText(this.DecimalGrade);
    }
}
