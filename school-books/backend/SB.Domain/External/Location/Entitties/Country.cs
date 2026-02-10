namespace SB.Domain;

public class Country
{
    // EF constructor
    private Country()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int CountryId { get; private set; }

    public string Name { get; private set; }
}
