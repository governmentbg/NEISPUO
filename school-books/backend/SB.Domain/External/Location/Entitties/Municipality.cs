namespace SB.Domain;

public class Municipality
{
    // EF constructor
    private Municipality()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int MunicipalityId { get; private set; }

    public int RegionId { get; private set; }

    public string Name { get; private set; }
}
