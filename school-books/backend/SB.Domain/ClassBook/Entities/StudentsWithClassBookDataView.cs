namespace SB.Domain;

public class StudentsWithClassBookDataView
{
    // EF constructor
    private StudentsWithClassBookDataView()
    {
        this.FirstName = null!;
        this.LastName = null!;
    }

    public int PersonId { get; private set; }

    public int SchoolYear { get; private set; }

    public int InstId { get; private set; }

    public int ClassBookId { get; private set; }

    public string FirstName { get; private set; }

    public string? MiddleName { get; private set; }

    public string LastName { get; private set; }
}
