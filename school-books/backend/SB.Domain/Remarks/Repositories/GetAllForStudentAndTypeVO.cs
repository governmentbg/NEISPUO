namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IRemarksQueryRepository
{
    public record GetAllForStudentAndTypeVO(
        int RemarkId,
        int CurriculumId,
        int PersonId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        DateTime Date,
        RemarkType Type,
        string TypeName,
        string Description,
        bool IsReadFromParent,
        DateTime CreateDate,
        int CreatedBySysUserId,
        string CreatedBySysUserFirstName,
        string? CreatedBySysUserMiddleName,
        string CreatedBySysUserLastName,
        DateTime ModifyDate,
        int ModifiedBySysUserId,
        string ModifiedBySysUserFirstName,
        string? ModifiedBySysUserMiddleName,
        string ModifiedBySysUserLastName,
        [property: JsonIgnore] int[] WriteAccessCurriculumTeacherPersonIds,
        [property: JsonIgnore] int[] ReplTeacherPersonIds)
    {
        public bool HasEditAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}
