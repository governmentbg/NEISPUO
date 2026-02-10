namespace SB.Domain;

public partial interface IInstitutionsQueryRepository
{
    public record GetProtocolTemplateDataVO(
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string? DirectorFirstName,
        string? DirectorMiddleName,
        string? DirectorLastName);
}
