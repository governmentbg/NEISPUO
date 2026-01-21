namespace SB.Domain;
public class TopicDplrStudent
{
    public TopicDplrStudent()
    {
    }

    public TopicDplrStudent(
        TopicDplr topicDplr,
        int personId)
    {
        this.TopicDplr = topicDplr;
        this.PersonId = personId;
    }

    public int SchoolYear { get; private set; }

    public int TopicDplrId { get; private set; }

    public int PersonId { get; private set; }

    public TopicDplr TopicDplr { get; private set; } = null!;
}
