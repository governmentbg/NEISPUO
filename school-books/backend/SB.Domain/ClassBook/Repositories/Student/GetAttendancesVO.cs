namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetAttendancesVO(
        DateTime Date,
        AttendanceType Type,
        string TypeName,
        string? ExcusedReasonName,
        string? ExcusedReasonComment);
}
