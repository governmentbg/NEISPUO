namespace SB.Domain;

using System;

public partial interface IRegBookQualificationDuplicateQueryRepository
{
    public record GetVO(
        string RegistrationNumberTotal,
        string? RegistrationNumberYear,
        DateTime? RegistrationDate,
        string FirstName,
        string? MiddleName,
        string LastName,
        string PersonalId,
        string BasicDocumentName,
        string? Series,
        string? FactoryNumber,
        string? ClassTypeName,
        string? SPPOOProfessionName,
        string? SPPOOSpecialityName,
        string? EduFormName,
        string? OrigSeries,
        string? OrigFactoryNumber,
        string? OrigRegistrationNumber,
        string? OrigRegistrationNumberYear,
        DateTime? OrigRegistrationDate,
        bool IsCancelled);
}
