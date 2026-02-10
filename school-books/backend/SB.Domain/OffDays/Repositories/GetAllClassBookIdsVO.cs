namespace SB.Domain;
public partial interface IOffDaysQueryRepository
{
    public record GetAllClassBooksVO(
        int SchoolYear,
        int ClassBookId,
        int? BasicClassId);
}
