namespace SB.Domain;

using System;

public record CreateSupportExtApiCommandActivity
{
    public int? SupportActivityTypeId { get; init; }
    public DateTime? Date { get; init; }
    public string? Target { get; init; }
    public string? Result { get; init; }
}
