namespace SB.Domain;

using System;

public partial interface ISkillsCheckExamResultProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int SkillsCheckExamResultProtocolId,
        string? ProtocolNumber,
        int SubjectId,
        DateTime? Date,
        int StudentsCapacity);
}
