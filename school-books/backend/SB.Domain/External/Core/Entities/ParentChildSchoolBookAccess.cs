namespace SB.Domain;

public class ParentChildSchoolBookAccess
{
    // EF constructor
    private ParentChildSchoolBookAccess()
    {
    }

    // only used properties should be mapped
    public int ParentChildSchoolBookAccessId { get; private set; }

    public int ChildId { get; private set; }

    public int ParentId { get; private set; }

    public bool HasAccess { get; private set; }
}
