namespace SB.Domain;

using System;

public class SpbsBookRecordAbsence
{
    // EF constructor
    private SpbsBookRecordAbsence()
    {
        this.AbsenceReason = null!;
        this.SpbsBookRecord = null!;
    }

    internal SpbsBookRecordAbsence(
        SpbsBookRecord spbsBookRecord,
        int orderNum,
        DateTime absenceDate,
        string absenceReason)
    {
        this.SpbsBookRecord = spbsBookRecord;
        this.OrderNum = orderNum;
        this.AbsenceDate = absenceDate;
        this.AbsenceReason = absenceReason;
    }

    public int SchoolYear { get; private set; }

    public int SpbsBookRecordId { get; private set; }

    public int OrderNum { get; private set; }

    public DateTime AbsenceDate { get; private set; }

    public string AbsenceReason { get; private set; }

    // relations
    public SpbsBookRecord SpbsBookRecord { get; private set; }

    internal void Update(
        DateTime absenceDate,
        string absenceReason)
    {
        this.AbsenceDate = absenceDate;
        this.AbsenceReason = absenceReason;
    }
}
