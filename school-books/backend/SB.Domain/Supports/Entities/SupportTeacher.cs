namespace SB.Domain;

public class SupportTeacher
{
    // EF constructor
    private SupportTeacher()
    {
        this.Support = null!;
    }

    internal SupportTeacher(
        Support support,
        int PersonId)
    {
        this.Support = support;
        this.PersonId = PersonId;
    }

    public int SchoolYear { get; private set; }
    public int SupportId { get; private set; }
    public int PersonId { get; private set; }

    // relations
    public Support Support { get; private set; }
}
