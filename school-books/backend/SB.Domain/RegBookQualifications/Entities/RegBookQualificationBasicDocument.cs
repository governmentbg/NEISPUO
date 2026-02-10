namespace SB.Domain;

public class RegBookQualificationBasicDocument
{
    // EF constructor
    private RegBookQualificationBasicDocument()
    {
        this.Name = null!;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
}
