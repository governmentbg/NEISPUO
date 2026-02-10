namespace SB.Domain;

public partial interface IOffDaysQueryRepository
{
    public record GetAllClassBookNamesVO(
        int ClassBookId,
        string FullBookName);
}
