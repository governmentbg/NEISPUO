namespace SB.Domain;

using System;

public partial interface IAttendancesQueryRepository
{
    public record GetAllForMonthVO(
        int AttendanceId,
        int PersonId,
        AttendanceType Type,
        DateTime Date);
}
