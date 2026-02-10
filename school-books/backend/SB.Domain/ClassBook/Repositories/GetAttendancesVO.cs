namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetAttendancesVO(
        int PersonId,
        AttendanceType Type,
        DateTime Date);
}
