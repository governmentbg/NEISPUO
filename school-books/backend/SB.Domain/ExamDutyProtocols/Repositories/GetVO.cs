namespace SB.Domain;

using System;

public partial interface IExamDutyProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int ExamDutyProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string? SessionType,
        int SubjectId,
        int SubjectTypeId,
        int? EduFormId,
        int ProtocolExamTypeId,
        int ProtocolExamSubTypeId,
        string OrderNumber,
        DateTime OrderDate,
        DateTime Date,
        string? GroupNum,
        int[] ClassIds,
        int[] SupervisorPersonIds);
}
