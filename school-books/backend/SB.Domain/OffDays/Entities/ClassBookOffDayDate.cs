namespace SB.Domain;

using System;

public class ClassBookOffDayDate : IAggregateRoot
{
    // EF constructor
    private ClassBookOffDayDate()
    {
    }

    public ClassBookOffDayDate(
        int schoolYear,
        int classBookId,
        DateTime date,
        int offDayId,
        bool isPgOffProgramDay)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Date = date;
        this.OffDayId = offDayId;
        this.IsPgOffProgramDay = isPgOffProgramDay;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public DateTime Date { get; private set; }

    public int OffDayId { get; private set; }

    public bool IsPgOffProgramDay { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public void UpdateData(int offDayId, bool isPgOffProgramDay)
    {
        this.OffDayId = offDayId;
        this.IsPgOffProgramDay = isPgOffProgramDay;
    }
}
