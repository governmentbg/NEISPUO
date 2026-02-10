namespace SB.Domain;

public class CurrentYear
{
    // EF constructor
    private CurrentYear()
    {
    }

    // only used properties should be mapped

    public int CurrentYearId { get; private set; }

    public bool IsValid { get; private set; }
}
