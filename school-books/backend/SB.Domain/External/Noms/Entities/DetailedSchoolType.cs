namespace SB.Domain;

public class DetailedSchoolType
{
    // EF constructor
    private DetailedSchoolType()
    {
    }

    // only used properties should be mapped

    public int DetailedSchoolTypeId { get; private set; }

    public int BaseSchoolTypeId { get; private set; }

    public InstType InstType { get; private set; }
}
