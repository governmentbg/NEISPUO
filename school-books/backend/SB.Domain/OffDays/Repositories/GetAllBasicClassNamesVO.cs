namespace SB.Domain;
public partial interface IOffDaysQueryRepository
{
    public record GetAllBasicClassNamesVO(
        int BasicClassId,
        string Name);
}
