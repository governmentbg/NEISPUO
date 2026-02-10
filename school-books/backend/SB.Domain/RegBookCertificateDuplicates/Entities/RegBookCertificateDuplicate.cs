namespace SB.Domain;

using System;

public class RegBookCertificateDuplicate
{
    // EF constructor
    private RegBookCertificateDuplicate()
    {
        this.RegistrationNumberTotal = null!;
    }

    public int Id { get; private set; }

    public int InstitutionId { get; private set; }

    public int SchoolYear { get; private set; }

    public string RegistrationNumberTotal { get; private set; }

    public string? RegistrationNumberYear { get; private set; }

    public DateTime? RegistrationDate { get; private set; }

    public int PersonId { get; private set; }

    public int BasicDocumentId { get; private set; }

    public string? OrigRegistrationNumber { get; private set; }

    public string? OrigRegistrationNumberYear { get; private set; }

    public DateTime? OrigRegistrationDate { get; private set; }

    public bool IsCancelled { get; private set; }
}
