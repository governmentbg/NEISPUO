namespace SB.Domain;

public class BudgetingInstitution
{
    // EF constructor
    private BudgetingInstitution()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int BudgetingInstitutionId { get; private set; }

    public string Name { get; private set; }
}
