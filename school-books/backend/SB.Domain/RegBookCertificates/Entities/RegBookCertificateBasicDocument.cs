namespace SB.Domain;

public class RegBookCertificateBasicDocument
{
    // EF constructor
    private RegBookCertificateBasicDocument()
    {
        this.Name = null!;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
}
