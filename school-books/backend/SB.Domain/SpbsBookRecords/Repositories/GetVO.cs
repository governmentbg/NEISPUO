namespace SB.Domain;

using System;

public partial interface ISpbsBookRecordsQueryRepository
{
    public record GetVO(
        GetVOStudent StudentPersonalInfo,
        GetVORelative[] Relatives,
        string? SendingCommission,
        string? SendingCommissionAddress,
        string? SendingCommissionPhoneNumber,
        string? InspectorNames,
        string? InspectorAddress,
        string? InspectorPhoneNumber,
        GetVOMovement Movement);

    public record GetVOStudent(
        int SchoolYear,
        int RecordNumber,
        string PersonalId,
        string FirstName,
        string? MiddleName,
        string LastName,
        string Gender,
        string? BirthPlaceCountry,
        string? BirthPlaceTown,
        string? PermanentTown,
        string? PermanentAddress,
        string ClassName);

    public record GetVORelative(
        string Name,
        string Telephone,
        string Email,
        string PermanentAddress);

    public record GetVOMovement(
        int OrderNum,
        string? CourtDecisionNumber,
        DateTime? CourtDecisionDate,
        string? IncomingInst,
        string? IncommingLetterNumber,
        DateTime? IncommingLetterDate,
        DateTime? IncommingDate,
        string? IncommingDocNumber,
        string? TransferInst,
        string? TransferReason,
        string? TransferProtocolNumber,
        DateTime? TransferProtocolDate,
        string? TransferLetterNumber,
        DateTime? TransferLetterDate,
        string? TransferCertificateNumber,
        DateTime? TransferCertificateDate,
        string? TransferMessageNumber,
        DateTime? TransferMessageDate);
}
