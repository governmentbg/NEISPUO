namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentCoverPageDataVO(
        string StudentName,
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
        string? ClassSpecialityName
    );
}
