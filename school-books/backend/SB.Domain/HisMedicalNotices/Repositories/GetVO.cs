namespace SB.Domain;

using System;

public partial interface IHisMedicalNoticesQueryRepository
{
    public record GetVO(
        string SchoolYears,
        string NrnMedicalNotice,
        string NrnExamination,
        int IdentifierType,
        string Identifier,
        string GivenName,
        string FamilyName,
        string Pmi,
        string? MatchedStudentNames,
        string? MatchedStudentIdentifier,
        DateTime AuthoredOn,
        DateTime FromDate,
        DateTime ToDate,
        GetVOReceipt[] Receipts);

    public record GetVOReceipt(
        string ExtSystemName,
        DateTime CreateDate,
        DateTime? AcknowledgedDate,
        string[] Accesses);
}
