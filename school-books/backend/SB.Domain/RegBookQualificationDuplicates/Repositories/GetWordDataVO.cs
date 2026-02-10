namespace SB.Domain;

using System;

public partial interface IRegBookQualificationDuplicateQueryRepository
{
    public record GetWordDataVO(
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionLocalAreaName,
        string InstitutionRegionName,
        string RegistrationNumberTotal,
        string BasicDocumentName,
        string? RegistrationNumberYear,
        DateTime? RegistrationDate,
        string FullName);
}
