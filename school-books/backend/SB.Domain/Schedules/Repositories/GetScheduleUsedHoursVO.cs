namespace SB.Domain;

public partial interface ISchedulesQueryRepository
{
    public record GetScheduleUsedHoursVO(int Day, int HourNumber);
}
