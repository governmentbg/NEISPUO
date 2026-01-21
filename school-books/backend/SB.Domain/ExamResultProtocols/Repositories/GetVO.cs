namespace SB.Domain;

using System;

public partial interface IExamResultProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int ExamResultProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string? SessionType,
        int SubjectId,
        int SubjectTypeId,
        string? GroupNum,
        int[] ClassIds,
        int? EduFormId,
        int ProtocolExamTypeId,
        int ProtocolExamSubTypeId,
        DateTime Date,
        string CommissionNominationOrderNumber,
        DateTime CommissionNominationOrderDate,
        int CommissionChairman,
        int[] CommissionMembers);
}
