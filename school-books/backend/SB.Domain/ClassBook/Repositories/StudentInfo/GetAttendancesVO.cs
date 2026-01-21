namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetAttendancesVO(
        DateTime Date,
        AttendanceType Type,
        string TypeName,
        string? ExcusedReasonName,
        string? ExcusedReasonComment);
}
