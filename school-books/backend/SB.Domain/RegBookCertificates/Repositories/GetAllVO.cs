namespace SB.Domain;

using System;

public partial interface IRegBookCertificateQueryRepository
{
    public record GetAllVO(
        int Id,
        string? RegistrationNumberTotal,
        string? RegistrationNumberYear,
        DateTime? RegistrationDate,
        string FullName,
        string PersonalId,
        string BasicDocumentName,
        bool IsCancelled);
}
