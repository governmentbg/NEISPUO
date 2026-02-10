namespace SB.Domain;

using System;

public partial interface IStateExamDutyProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int StateExamDutyProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string? SessionType,
        int SubjectId,
        int SubjectTypeId,
        int? EduFormId,
        string OrderNumber,
        DateTime OrderDate,
        DateTime Date,
        int ModulesCount,
        string? RoomNumber,
        int[] SupervisorPersonIds);
}
