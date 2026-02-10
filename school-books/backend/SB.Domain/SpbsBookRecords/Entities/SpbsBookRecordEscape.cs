namespace SB.Domain;

using System;

public class SpbsBookRecordEscape
{
    // EF constructor
    private SpbsBookRecordEscape()
    {
        this.PoliceLetterNumber = null!;
        this.SpbsBookRecord = null!;
    }

    internal SpbsBookRecordEscape(
        SpbsBookRecord spbsBookRecord,
        int orderNum,
        DateTime escapeDate,
        TimeSpan escapeTime,
        DateTime policeNotificationDate,
        TimeSpan policeNotificationTime,
        string policeLetterNumber,
        DateTime policeLetterDate,
        DateTime? returnDate)
    {
        this.SpbsBookRecord = spbsBookRecord;
        this.OrderNum = orderNum;
        this.EscapeDate = escapeDate;
        this.EscapeTime = escapeTime;
        this.PoliceNotificationDate = policeNotificationDate;
        this.PoliceNotificationTime = policeNotificationTime;
        this.PoliceLetterNumber = policeLetterNumber;
        this.PoliceLetterDate = policeLetterDate;
        this.ReturnDate = returnDate;
    }

    public int SchoolYear { get; private set; }

    public int SpbsBookRecordId { get; private set; }

    public int OrderNum { get; private set; }

    public DateTime EscapeDate { get; private set; }

    public TimeSpan EscapeTime { get; private set; }

    public DateTime PoliceNotificationDate { get; private set; }

    public TimeSpan PoliceNotificationTime { get; private set; }

    public string PoliceLetterNumber { get; private set; }

    public DateTime PoliceLetterDate { get; private set; }

    public DateTime? ReturnDate { get; private set; }

    // relations
    public SpbsBookRecord SpbsBookRecord { get; private set; }

    internal void Update(
        DateTime escapeDate,
        TimeSpan escapeTime,
        DateTime policeNotificationDate,
        TimeSpan policeNotificationTime,
        string policeLetterNumber,
        DateTime policeLetterDate,
        DateTime? returnDate)
    {
        this.EscapeDate = escapeDate;
        this.EscapeTime = escapeTime;
        this.PoliceNotificationDate = policeNotificationDate;
        this.PoliceNotificationTime = policeNotificationTime;
        this.PoliceLetterNumber = policeLetterNumber;
        this.PoliceLetterDate = policeLetterDate;
        this.ReturnDate = returnDate;
    }
}
