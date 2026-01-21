namespace SB.Domain;

using System;

public partial interface INvoExamDutyProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int NvoExamDutyProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        int BasicClassId,
        int SubjectId,
        int SubjectTypeId,
        DateTime Date,
        string? RoomNumber,
        int DirectorPersonId,
        int[] SupervisorPersonIds);
}
