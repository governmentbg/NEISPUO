namespace SB.Domain;
public partial interface ITopicPlansQueryRepository
{
    public record GetVO(
        int TopicPlanId,
        string Name,
        int? BasicClassId,
        int? SubjectId,
        int? SubjectTypeId,
        int? TopicPlanPublisherId,
        string? TopicPlanPublisherOther);
}
