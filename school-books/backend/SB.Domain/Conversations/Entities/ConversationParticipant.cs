namespace SB.Domain;

using System;
using Data;

public class ConversationParticipant
{
    // Constructor for EF
    private ConversationParticipant()
    {
        this.Title = null!;
    }

    public ConversationParticipant(
        Conversation conversation,
        int schoolYear,
        int instId,
        int sysUserId,
        string title,
        ParticipantType participantType,
        ConversationParticipantGroup? conversationGroup = null,
        bool isCreator = false)
    {
        this.Conversation = conversation;
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.SysUserId = sysUserId;
        this.Title = title;
        this.ParticipantType = participantType;
        this.ParticipantGroup = conversationGroup;
        this.IsCreator = isCreator;
    }

    public int SchoolYear { get; private set; }
    public int InstId { get; set; }
    public int ConversationId { get; private set; }
    public int ConversationParticipantId { get; private set; }
    public int SysUserId { get; private set; }
    public string Title { get; private set; }
    public ParticipantType ParticipantType { get; set; }
    public int? ConversationParticipantGroupId { get; private set; }
    public int? LastReadMessageId { get; private set; }
    public DateTime? LastReadMessageDate { get; private set; }
    public bool IsCreator { get; private set; }

    // Navigation properties
    public Conversation Conversation { get; private set; } = null!;
    public ConversationParticipantGroup? ParticipantGroup { get; private set; }

    public void UpdateLastReadMessage(int messageId, DateTime messageDate)
    {
        this.LastReadMessageId = messageId;
        this.LastReadMessageDate = messageDate;
    }
}
