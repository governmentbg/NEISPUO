namespace SB.Domain;

using System;

public class TopicPlan : IAggregateRoot
{
    // EF constructor
    private TopicPlan()
    {
        this.Name = null!;
    }

    public TopicPlan(
        int sysUserId,
        string name,
        int? basicClassId,
        int? subjectId,
        int? subjectTypeId,
        int? topicPlanPublisherId,
        string? topicPlanPublisherOther)
    {
        this.CreatedBySysUserId = sysUserId;
        this.Name = name;
        this.BasicClassId = basicClassId;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.TopicPlanPublisherId = topicPlanPublisherId;
        this.TopicPlanPublisherOther = topicPlanPublisherOther;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
        this.Version = null!;
    }

    public int TopicPlanId { get; private set; }

    public string Name { get; private set; }

    public int? BasicClassId { get; private set; }

    public int? SubjectId { get; private set; }

    public int? SubjectTypeId { get; private set; }

    public int? TopicPlanPublisherId { get; private set; }

    public string? TopicPlanPublisherOther { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public void Update(
        string name,
        int? basicClassId,
        int? subjectId,
        int? subjectTypeId,
        int? topicPlanPublisherId,
        string? topicPlanPublisherOther)
    {
        this.Name = name;
        this.BasicClassId = basicClassId;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.TopicPlanPublisherId = topicPlanPublisherId;
        this.TopicPlanPublisherOther = topicPlanPublisherOther;

        this.ModifyDate = DateTime.Now;
    }
}
