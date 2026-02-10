namespace SB.Domain;

public class Region
{
    // EF constructor
    private Region()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int RegionId { get; private set; }

    public string Name { get; private set; }
}
