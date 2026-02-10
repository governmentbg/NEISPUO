namespace SB.Domain;

using System;
using System.Text.Json.Serialization;
using MediatR;

public record UpdateSpbsBookRecordCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? SpbsBookRecordId { get; init; }

    public string? SendingCommission { get; init; }
    public string? SendingCommissionAddress { get; init; }
    public string? SendingCommissionPhoneNumber { get; init; }
    public string? InspectorNames { get; init; }
    public string? InspectorAddress { get; init; }
    public string? InspectorPhoneNumber { get; init; }

    // movement
    public string? CourtDecisionNumber { get; init; }
    public DateTime? CourtDecisionDate { get; init; }
    public int? IncomingInstId { get; init; }
    public string? IncommingLetterNumber { get; init; }
    public DateTime? IncommingLetterDate { get; init; }
    public DateTime? IncommingDate { get; init; }
    public string? IncommingDocNumber { get; init; }
    public int? TransferInstId { get; init; }
    public string? TransferReason { get; init; }
    public string? TransferProtocolNumber { get; init; }
    public DateTime? TransferProtocolDate { get; init; }
    public string? TransferLetterNumber { get; init; }
    public DateTime? TransferLetterDate { get; init; }
    public string? TransferCertificateNumber { get; init; }
    public DateTime? TransferCertificateDate { get; init; }
    public string? TransferMessageNumber { get; init; }
    public DateTime? TransferMessageDate { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SpbsBookRecord);
    [JsonIgnore]public int? ObjectId => this.SpbsBookRecordId;
}
