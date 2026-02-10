namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetCoverPageDataVO(
        int? BasicClassId,
        ClassBookType BookType,
        string InstitutionAbbreviation,
        string? InstitutionTownTypeName,
        string? InstitutionTownName,
        string? InstitutionMunicipalityName,
        string? InstitutionLocalAreaName,
        string? InstitutionRegionName,
        string ClassName,
        string? SupportTypeName,
        string? ClassSpecialityName,
        string? SchoolYearProgram);
}
