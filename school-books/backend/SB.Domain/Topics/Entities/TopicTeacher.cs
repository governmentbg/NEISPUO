namespace SB.Domain;
public class TopicTeacher
{
    // EF constructor
    private TopicTeacher()
    {
    }

    public TopicTeacher(
        Topic topic,
        int personId,
        bool isReplTeacher)
    {
        this.Topic = topic;
        this.PersonId = personId;
        this.IsReplTeacher = isReplTeacher;
    }

    public int SchoolYear { get; private set; }

    public int TopicId { get; private set; }

    public int PersonId { get; private set; }

    public bool IsReplTeacher { get; private set; }

    // relations
    public Topic Topic { get; private set; } = null!;
}
