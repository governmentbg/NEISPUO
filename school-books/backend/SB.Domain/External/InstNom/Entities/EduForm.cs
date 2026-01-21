namespace SB.Domain;

public class EduForm
{
    // EF constructor
    private EduForm()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int ClassEduFormId { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }
}
