namespace SB.Domain;

public class SPPOOProfession
{
    // EF constructor
    private SPPOOProfession()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int SPPOOProfessionId { get; private set; }

    public string Name { get; private set; }
}
