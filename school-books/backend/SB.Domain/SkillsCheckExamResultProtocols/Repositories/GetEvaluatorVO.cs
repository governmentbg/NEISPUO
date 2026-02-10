namespace SB.Domain;
public partial interface ISkillsCheckExamResultProtocolsQueryRepository
{
    public record GetEvaluatorVO(
        int SkillsCheckExamResultProtocolEvaluatorId,
        string Name);
}
