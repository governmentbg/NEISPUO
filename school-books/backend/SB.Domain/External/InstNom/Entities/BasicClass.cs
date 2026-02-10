namespace SB.Domain;

public class BasicClass
{
    // EF constructor
    private BasicClass()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int BasicClassId { get; private set; }

    public string Name { get; private set; }

    public int? SortOrd { get; private set; }

    public bool? IsValid { get; private set; }
}
