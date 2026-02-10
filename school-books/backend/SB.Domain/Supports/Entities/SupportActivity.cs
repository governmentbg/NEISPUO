namespace SB.Domain;

using System;

public class SupportActivity
{
    // EF constructor
    private SupportActivity()
    {
        this.Support = null!;
    }

    internal SupportActivity(
        Support support,
        int supportActivityTypeId,
        string? target,
        string? result,
        DateTime? date)
    {
        this.Support = support;
        this.SupportActivityTypeId = supportActivityTypeId;
        this.Target = target;
        this.Result = result;
        this.Date = date;
    }

    public int SchoolYear { get; private set; }
    public int SupportId { get; private set; }
    public int SupportActivityId { get; private set; }
    public int SupportActivityTypeId { get; private set; }
    public string? Target { get; private set; }
    public string? Result { get; private set; }
    public DateTime? Date { get; private set; }

    // relations
    public Support Support { get; private set; }

    public void UpdateData(
        int supportActivityTypeId,
        string? target,
        string? result,
        DateTime? date)
    {
        this.SupportActivityTypeId = supportActivityTypeId;
        this.Target = target;
        this.Result = result;
        this.Date = date;
    }
}
