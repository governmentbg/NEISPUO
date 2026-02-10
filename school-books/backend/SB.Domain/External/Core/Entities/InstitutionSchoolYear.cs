namespace SB.Domain;

public class InstitutionSchoolYear
{
    // EF constructor
    private InstitutionSchoolYear()
    {
        this.Name = null!;
        this.Abbreviation = null!;
    }

    // only used properties should be mapped

    public int InstitutionId { get; private set; }

    public int SchoolYear { get; private set; }

    public bool IsFinalized { get; private set; }

    public string Name { get; private set; }

    public string Abbreviation { get; private set; }

    public int? LocalAreaId { get; private set; }

    public int? TownId { get; private set; }

    public int DetailedSchoolTypeId { get; private set; }

    public bool IsCurrent { get; private set; }
}
