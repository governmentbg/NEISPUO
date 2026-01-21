namespace SB.Domain;

public class BasicDocument
{
    // EF constructor
    private BasicDocument()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int Id { get; private set; }

    public string Name { get; private set; }
}
