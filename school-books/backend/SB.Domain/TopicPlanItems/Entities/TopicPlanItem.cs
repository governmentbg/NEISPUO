namespace SB.Domain;

using System;

public class TopicPlanItem : IAggregateRoot
{
    // EF constructor
    private TopicPlanItem()
    {
        this.Version = null!;
        this.Title = null!;
    }

    internal TopicPlanItem(
        int topicPlanId,
        int number,
        string title,
        string? note)
    {
        this.TopicPlanId = topicPlanId;
        this.Number = number;
        this.Title = title;
        this.Note = note;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
        this.Version = null!;
    }

    public int TopicPlanId { get; private set; }
    public int TopicPlanItemId { get; private set; }
    public int Number { get; private set; }
    public string Title { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreateDate { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public byte[] Version { get; private set; }

    public void Update(
        int number,
        string title,
        string? note)
    {
        this.Number = number;
        this.Title = title;
        this.Note = note;

        this.ModifyDate = DateTime.Now;
    }
}
