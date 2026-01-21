namespace SB.Domain;

public class Relative
{
    // EF constructor
    private Relative()
    {
    }

    // only used properties should be mapped

    public int RelativeId { get; private set; }

    public int PersonId { get; private set; }

    public string? Email { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string? FirstName { get; private set; }

    public string? MiddleName { get; private set; }

    public string? LastName { get; private set; }
}
