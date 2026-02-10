namespace SB.Domain;

public partial interface ITopicsQueryRepository
{
    public record GetCurriculumTopiPlanVO(
        int ClassBookTopicPlanItemId,
        int Number,
        string Title,
        bool Taken);
}
