namespace SB.Domain;
public partial interface ISchoolYearSettingsQueryRepository
{
    public record GetAllClassBooksVO(
        int SchoolYear,
        int ClassBookId,
        int? BasicClassId,
        int?[] ChildBasicClassIds);
}
