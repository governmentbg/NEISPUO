namespace SB.Domain;

using System;

public partial interface IRegBookCertificateDuplicateQueryRepository
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
        string? OrigRegistrationNumber,
        string? OrigRegistrationNumberYear,
        DateTime? OrigRegistrationDate);
}
