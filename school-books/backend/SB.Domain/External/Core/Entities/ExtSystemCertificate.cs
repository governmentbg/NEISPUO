namespace SB.Domain;

public class ExtSystemCertificate
{
    // EF constructor
    private ExtSystemCertificate()
    {
        this.Thumbprint = null!;
    }

    // only used properties should be mapped

    public int ExtSystemId { get; private set; }
    public string Thumbprint { get; private set; }
    public bool IsValid { get; private set; }
}
