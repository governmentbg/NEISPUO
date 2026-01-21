namespace SB.Domain;

public class Institution
{
    // EF constructor
    private Institution()
    {
        this.Name = null!;
        this.Abbreviation = null!;
    }

    // only used properties should be mapped

    public int InstitutionId { get; private set; }

    public string Name { get; private set; }

    public string Abbreviation { get; private set; }

    public int DetailedSchoolTypeId { get; private set; }

    public int BudgetingSchoolTypeId { get; private set; }
}
