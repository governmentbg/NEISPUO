namespace SB.Domain;

public class LocalArea
{
    // EF constructor
    private LocalArea()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int LocalAreaId { get; private set; }

    public string Name { get; private set; }
}
