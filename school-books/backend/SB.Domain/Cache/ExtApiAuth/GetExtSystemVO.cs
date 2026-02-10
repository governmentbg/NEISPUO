namespace SB.Domain;

public partial interface IExtApiAuthQueryRepository
{
    public record GetExtSystemVO(
        int ExtSystemId,
        int[] ExtSystemTypes,
        int? SysUserId);
}
