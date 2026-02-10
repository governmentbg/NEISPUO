namespace SB.Domain;

public class InstitutionDepartment
{
    // EF constructor
    private InstitutionDepartment()
    {
    }

    // only used properties should be mapped

    public int InstitutionDepartmentId { get; private set; }

    public int InstitutionId { get; private set; }

    public int? TownId { get; private set; }

    public bool? IsMain { get; private set; }
}
