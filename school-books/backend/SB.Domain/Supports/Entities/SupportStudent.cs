namespace SB.Domain;

public class SupportStudent
{
    // EF constructor
    private SupportStudent()
    {
        this.Support = null!;
    }

    internal SupportStudent(
        Support support,
        int personId)
    {
        this.Support = support;
        this.PersonId = personId;
    }

    public int SchoolYear { get; private set; }
    public int SupportId { get; private set; }
    public int PersonId { get; private set; }

    // relations
    public Support Support { get; private set; }
}
