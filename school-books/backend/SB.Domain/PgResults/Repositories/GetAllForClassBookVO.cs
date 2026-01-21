namespace SB.Domain;
public partial interface IPgResultsQueryRepository
{
    public record GetAllForClassBookVO(
        int PersonId,
        int Count);
}
