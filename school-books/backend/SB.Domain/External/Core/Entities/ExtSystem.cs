namespace SB.Domain;

public class ExtSystem
{
    // EF constructor
    private ExtSystem()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int ExtSystemId { get; private set; }
    public string Name { get; private set; }
    public int? SysUserId { get; private set; }
    public bool IsValid { get; private set; }
}
