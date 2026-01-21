namespace SB.Domain;

public class OffDayClassBook
{
    // EF constructor
    public OffDayClassBook()
    {
        this.OffDay = null!;
    }

    public OffDayClassBook(OffDay offDay, int classBookId)
    {
        this.OffDay = offDay;
        this.ClassBookId = classBookId;
    }

    public int SchoolYear { get; private set; }

    public int OffDayId { get; private set; }

    public int ClassBookId { get; private set; }

    // relations
    public OffDay OffDay { get; private set; }
}
