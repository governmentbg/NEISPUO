namespace SB.Domain;

using System;

public partial interface IAbsencesDplrQueryRepository
{
    public record GetAllDplrForStudentAndTypeVO(
        int AbsenceId,
        int PersonId,
        int ScheduleLessonId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        DateTime Date,
        AbsenceType Type,
        string TypeName,
        bool? ReplTeacherIsNonSpecialist,
        DateTime CreateDate,
        int CreatedBySysUserId,
        string CreatedBySysUserFirstName,
        string? CreatedBySysUserMiddleName,
        string CreatedBySysUserLastName)
    {
        public bool HasUndoAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}
