namespace SB.Domain;

public class InstitutionConfData
{
    // EF constructor
    private InstitutionConfData()
    {
    }

    // only used properties should be mapped

    public int InstitutionConfDataID { get; private set; }
    public int InstitutionId { get; private set; }
    public int SchoolYear { get; private set; }
}
