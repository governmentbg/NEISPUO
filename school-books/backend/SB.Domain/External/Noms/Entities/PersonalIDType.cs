namespace SB.Domain;

public class PersonalIdType
{
    // EF constructor
    private PersonalIdType()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int PersonalIdTypeId { get; private set; }

    public string Name { get; private set; }
}
