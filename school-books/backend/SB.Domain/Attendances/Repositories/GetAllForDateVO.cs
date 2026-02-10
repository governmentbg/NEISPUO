namespace SB.Domain;

using System;

public partial interface IAttendancesQueryRepository
{
    public record GetAllForDateVO(
        int AttendanceId,
        int PersonId,
        AttendanceType Type,
        DateTime Date);
}
