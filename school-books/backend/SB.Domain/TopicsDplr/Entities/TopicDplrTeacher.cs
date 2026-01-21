namespace SB.Domain;
public class TopicDplrTeacher
{
    // EF constructor
    private TopicDplrTeacher()
    {
    }

    public TopicDplrTeacher(
        TopicDplr topicDplr,
        int personId)
    {
        this.TopicDplr = topicDplr;
        this.PersonId = personId;
    }

    public int SchoolYear { get; private set; }

    public int TopicDplrId { get; private set; }

    public int PersonId { get; private set; }

    // relations
    public TopicDplr TopicDplr { get; private set; } = null!;
}
