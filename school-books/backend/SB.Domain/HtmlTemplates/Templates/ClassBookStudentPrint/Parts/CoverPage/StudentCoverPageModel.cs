namespace SB.Domain;

public record StudentCoverPageModel(
    string StudentName,
    int SchoolYear,
    string InstitutionAbbreviation,
    string? InstitutionTownTypeName,
    string? InstitutionTownName,
    string? InstitutionMunicipalityName,
    string? InstitutionLocalAreaName,
    string? InstitutionRegionName,
    string ClassName,
    string? SupportTypeName,
    string? ClassSpecialityName
);
