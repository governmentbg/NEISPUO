namespace SB.Domain;

using System;

public partial interface IAbsencesQueryRepository
{
    public record GetAllForStudentAndTypeVO(
        int AbsenceId,
        int PersonId,
        int ScheduleLessonId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        DateTime Date,
        AbsenceType Type,
        string TypeName,
        int? ExcusedReasonId,
        string? ExcusedReasonName,
        string? ExcusedReasonComment,
        bool? ReplTeacherIsNonSpecialist,
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
        string ModifiedBySysUserLastName)
    {
        public bool HasExcuseAccess { get; set; } // should be mutable

        public bool HasUndoAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}
