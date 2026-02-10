namespace SB.Domain;

public class PersonDetail
{
    // EF constructor
    private PersonDetail()
    {
    }

    // only used properties should be mapped

    public int PersonDetailId { get; set; }

    public int PersonId { get; private set; }

    public string? PhoneNumber { get; private set; }
}
