namespace SB.Domain;

using System;

public partial interface IRegBookQualificationQueryRepository
{
    public record GetVO(
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
        GetVODuplicate[] Duplicates);

    public record GetVODuplicate(
        int Id,
        DateTime? RegistrationDate,
        string NumberTotal);
}
