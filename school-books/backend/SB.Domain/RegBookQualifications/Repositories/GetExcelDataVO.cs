namespace SB.Domain;

using System;

public partial interface IRegBookQualificationQueryRepository
{
    public record GetExcelDataVO(
        int Id,
        string RegistrationNumberTotal,
        string? RegistrationNumberYear,
        DateTime? RegistrationDate,
        string FirstName,
        string? MiddleName,
        string LastName,
        string PersonalId,
        string BasicDocumentName,
        string? EduFormName,
        string? ClassTypeName,
        string? SPPOOProfessionName,
        string? SPPOOSpecialityName,
        string Gpa,
        string? Series,
        string? FactoryNumber,
        bool IsCancelled,
        GetExcelDataVODuplicate[] Duplicates);

    public record GetExcelDataVODuplicate(
        DateTime? RegistrationDate,
        string DocumentSeriesAndNumber);
}
