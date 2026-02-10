namespace SB.Domain;
public partial interface ISkillsCheckExamResultProtocolsQueryRepository
{
    public record GetEvaluatorAllVO(
        int SkillsCheckExamResultProtocolEvaluatorId,
        string Name);
}
