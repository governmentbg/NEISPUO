namespace SB.Domain;

public partial interface ICommonCQSQueryRepository
{
    public record GetAllExtProvidersVO(
        int InstId,
        int? ExtSystemId,
        int? ScheduleExtSystemId);
}
