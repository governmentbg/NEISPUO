namespace SB.Domain;

public class RegBookCertificateDuplicateBasicDocument
{
    // EF constructor
    private RegBookCertificateDuplicateBasicDocument()
    {
        this.Name = null!;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
}
