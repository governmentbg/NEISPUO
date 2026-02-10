namespace SB.Domain;

using System;

public partial interface IAttendancesQueryRepository
{
    public record GetVO(
        int AttendanceId,
        int PersonId,
        DateTime Date,
        AttendanceType Type,
        int? ExcusedReasonId,
        string? ExcusedReasonName,
        string? ExcusedReasonComment,
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
