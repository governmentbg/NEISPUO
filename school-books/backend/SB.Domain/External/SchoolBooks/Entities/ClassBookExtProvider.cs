namespace SB.Domain;

// this table is in the school_books schema but is
// maintained and modified by AdminSoft (institutions module)
public class ClassBookExtProvider
{
    // EF constructor
    private ClassBookExtProvider()
    {
    }

    public int SchoolYear { get; private set; }

    public int InstId { get; private set; }

    public int? ExtSystemId { get; private set; }

    public int? ScheduleExtSystemId { get; private set; }
}
