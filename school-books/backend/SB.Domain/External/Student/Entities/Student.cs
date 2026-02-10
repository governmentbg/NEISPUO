namespace SB.Domain;
public class Student
{
    // EF constructor
    private Student()
    {
    }

    // only used properties should be mapped

    public int PersonId { get; private set; }

    public string? HomePhone { get; private set; }

    public string? WorkPhone { get; private set; }

    public string? MobilePhone { get; private set; }

    public string? GPPhone { get; private set; }

    public string? GPName { get; private set; }
}
