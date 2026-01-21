namespace SB.Domain;

public class Town
{
    // EF constructor
    private Town()
    {
        this.Name = null!;
        this.Type = null!;
    }

    // only used properties should be mapped

    public int TownId { get; private set; }

    public int MunicipalityId { get; private set; }

    public string Name { get; private set; }

    public string Type { get; private set; }
}
