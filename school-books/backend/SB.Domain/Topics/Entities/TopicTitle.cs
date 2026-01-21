namespace SB.Domain;
public class TopicTitle
{
    // EF constructor
    private TopicTitle()
    {
        this.Title = null!;
    }

    public TopicTitle(
        Topic topic,
        int index,
        string title,
        int? classBookTopicPlanItemId)
    {
        this.Topic = topic;
        this.Index = index;
        this.Title = title;
        this.ClassBookTopicPlanItemId = classBookTopicPlanItemId;
    }

    public int SchoolYear { get; private set; }

    public int TopicId { get; private set; }

    public int Index { get; private set; }

    public string Title { get; private set; }

    public int? ClassBookTopicPlanItemId { get; private set; }

    // relations
    public Topic Topic { get; private set; } = null!;

    public void ClearClassBookTopicPlanItemId()
    {
        this.ClassBookTopicPlanItemId = null;
    }
}
