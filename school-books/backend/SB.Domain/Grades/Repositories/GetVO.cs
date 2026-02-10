namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public partial interface IGradesQueryRepository
{
    [Keyless]
    public record GetVO(
        int GradeId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        DateTime Date,
        GradeType Type,
        SchoolTerm Term,
        GradeCategory Category,
        [property: Column(TypeName = "DECIMAL(3,2)")] decimal? DecimalGrade,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade,
        string? Comment,
        DateTime CreateDate,
        int CreatedBySysUserId,
        string CreatedBySysUserFirstName,
        string? CreatedBySysUserMiddleName,
        string CreatedBySysUserLastName,
        bool IsReadFromParent)
    {
        [NotMapped]
        public bool HasUndoAccess { get; set; } // should be mutable

        [NotMapped]
        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}
