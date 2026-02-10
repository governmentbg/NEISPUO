namespace SB.Domain;

using System;

public class ConversationMessage
{
    // Constructor for EF
    private ConversationMessage()
    {
        this.Content = null!;
    }

    public ConversationMessage(
        int schoolYear,
        Conversation conversation,
        int createdByParticipantId,
        string content,
        DateTime createDate)
    {
        this.SchoolYear = schoolYear;
        this.Conversation = conversation;
        this.CreatedByParticipantId = createdByParticipantId;
        this.Content = content;
        this.CreateDate = createDate;
    }

    public int SchoolYear { get; private set; }
    public int ConversationId { get; private set; }
    public int ConversationMessageId { get; private set; }
    public int CreatedByParticipantId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreateDate { get; private set; }

    // relations
    public Conversation Conversation { get; private set; } = null!;
}
