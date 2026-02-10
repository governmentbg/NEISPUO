namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetShiftHoursVO(
        int ShiftId,
        int HourNumber,
        TimeSpan StartTime,
        TimeSpan EndTime
    );
}
