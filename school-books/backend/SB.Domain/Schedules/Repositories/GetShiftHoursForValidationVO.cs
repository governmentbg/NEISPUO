namespace SB.Domain;

public partial interface ISchedulesQueryRepository
{
    public record GetShiftHoursForValidationVO(
        int Day,
        int HourNumber);
}
