namespace SB.Domain;

public class CurriculumClass
{
    // EF constructor
    private CurriculumClass()
    {
    }

    // only used properties should be mapped

    public int CurriculumId { get; private set; }

    public int ClassId { get; private set; }

    public bool IsValid { get; private set; }
}
