namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IMyHourQueryRepository
{
    public record GetTeacherLessonAbsencesVO(
        int AbsenceId,
        int PersonId,
        AbsenceType Type,
        string TypeName,
        int? ExcusedReasonId,
        string? ExcusedReasonName,
        string? ExcusedReasonComment,
        DateTime CreateDate,
        [property: JsonIgnore] int CreatedBySysUserId,
        string CreatedBySysUserFirstName,
        string? CreatedBySysUserMiddleName,
        string CreatedBySysUserLastName,
        DateTime ModifyDate,
        [property: JsonIgnore] int ModifiedBySysUserId,
        string ModifiedBySysUserFirstName,
        string? ModifiedBySysUserMiddleName,
        string ModifiedBySysUserLastName)
    {
        public bool HasExcuseAccess { get; set; } // should be mutable

        public bool HasUndoAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}
