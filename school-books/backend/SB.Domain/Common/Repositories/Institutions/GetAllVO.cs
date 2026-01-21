namespace SB.Domain;

public partial interface IInstitutionsQueryRepository
{
    public record GetAllVO(
        int InstitutionId,
        int InstitutionSchoolYear,
        string InstitutionName,
        string TownName);
}
