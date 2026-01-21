namespace SB.Domain;

public class Gender
{
    // EF constructor
    private Gender()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int GenderId { get; private set; }

    public string Name { get; private set; }
}
