namespace SB.Domain;

using System;

public partial interface IExamDutyProtocolsQueryRepository
{
    public record GetAllVO(
        int ExamDutyProtocolId,
        int SchoolYear,
        string? OrderNumber,
        DateTime? OrderDate,
        DateTime? ProtocolDate,
        string? SessionType,
        ExamDutyProtocolType ProtocolType,
        string ProtocolTypeName);
}
