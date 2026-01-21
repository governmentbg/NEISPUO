namespace SB.Domain;

public class OffDayClass
{
    // EF constructor
    public OffDayClass()
    {
        this.OffDay = null!;
    }

    public OffDayClass(OffDay offDay, int basicClassId)
    {
        this.OffDay = offDay;
        this.BasicClassId = basicClassId;
    }

    public int SchoolYear { get; private set; }

    public int OffDayId { get; private set; }

    public int BasicClassId { get; private set; }

    // relations
    public OffDay OffDay { get; private set; }
}
