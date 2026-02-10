namespace SB.Domain;

public class ExtSystemAccess
{
    // EF constructor
    private ExtSystemAccess()
    {
    }

    // only used properties should be mapped

    public int ExtSystemAccessId { get; set; }
    public int ExtSystemId { get; private set; }
    public int ExtSystemType { get; private set; }
    public bool IsValid { get; private set; }
}
