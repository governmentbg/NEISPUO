namespace SB.Domain;

public class CurriculumStudent
{
    // EF constructor
    private CurriculumStudent()
    {
    }

    // only used properties should be mapped

    public int CurriculumId { get; private set; }

    public int PersonId { get; private set; }

    public bool IsValid { get; private set; }
}
