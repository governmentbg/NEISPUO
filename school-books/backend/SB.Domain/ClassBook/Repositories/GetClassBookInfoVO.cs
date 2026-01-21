namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetClassBookInfoVO(
        int InstId,
        int ClassId,
        bool ClassIsLvl2);
}
