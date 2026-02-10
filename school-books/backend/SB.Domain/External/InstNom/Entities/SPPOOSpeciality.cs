namespace SB.Domain;

public class SPPOOSpeciality
{
    // EF constructor
    private SPPOOSpeciality()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int SPPOOSpecialityId { get; private set; }

    public string Name { get; private set; }
}
