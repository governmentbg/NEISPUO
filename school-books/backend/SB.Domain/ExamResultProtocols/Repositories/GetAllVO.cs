namespace SB.Domain;

using System;

public partial interface IExamResultProtocolsQueryRepository
{
    public record GetAllVO(
        int ExamResultProtocolId,
        string? OrderNumber,
        ExamResultProtocolType? ProtocolType,
        string? ProtocolTypeName,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string SessionType);
}
