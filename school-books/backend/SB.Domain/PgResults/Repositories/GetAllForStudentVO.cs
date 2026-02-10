namespace SB.Domain;

using System;

public partial interface IPgResultsQueryRepository
{
    public record GetAllForStudentVO(
        int PgResultId,
        int? SubjectId,
        int PersonId,
        string SubjectName,
        string? SubjectNameShort,
        string? StartSchoolYearResult,
        string? EndSchoolYearResult,
        DateTime CreateDate,
        int CreatedBySysUserId,
        string CreatedBySysUserFirstName,
        string? CreatedBySysUserMiddleName,
        string CreatedBySysUserLastName,
        DateTime ModifyDate,
        int ModifiedBySysUserId,
        string ModifiedBySysUserFirstName,
        string? ModifiedBySysUserMiddleName,
        string ModifiedBySysUserLastName)
    {
        public bool HasEditAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}
