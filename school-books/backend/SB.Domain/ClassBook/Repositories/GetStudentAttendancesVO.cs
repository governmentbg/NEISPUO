namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentAttendancesVO(
        AttendanceType Type,
        DateTime Date
    );
}
