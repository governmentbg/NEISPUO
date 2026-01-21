namespace SB.Domain;

public class RegBookQualificationDuplicateBasicDocument
{
    // EF constructor
    private RegBookQualificationDuplicateBasicDocument()
    {
        this.Name = null!;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
}
