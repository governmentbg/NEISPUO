namespace SB.Domain;
public partial interface ITopicPlansQueryRepository
{
    public record GetAllVO(
        int TopicPlanId,
        string Name,
        string? BasicClassName,
        string? SubjectName,
        string? SubjectTypeName);
}
