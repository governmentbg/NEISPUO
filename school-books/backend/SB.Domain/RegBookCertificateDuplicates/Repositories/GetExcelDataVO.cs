namespace SB.Domain;

using System;

public partial interface IRegBookCertificateDuplicateQueryRepository
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
        string? OrigRegistrationNumberYear,
        DateTime? OrigRegistrationDate);
}
