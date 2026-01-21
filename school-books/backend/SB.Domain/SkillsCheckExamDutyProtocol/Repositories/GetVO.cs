namespace SB.Domain;

using System;

public partial interface ISkillsCheckExamDutyProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int SkillsCheckExamDutyProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        int SubjectId,
        int SubjectTypeId,
        DateTime Date,
        int DirectorPersonId,
        int StudentsCapacity,
        int[] SupervisorPersonIds);
}
