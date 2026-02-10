namespace SB.Domain;

using System.Collections.Generic;
using System;
using System.Linq;

public class Conversation : IAggregateRoot
{
    // Constructor for EF
    private Conversation()
    {
        this.Title = null!;
    }

    public Conversation(
        int schoolYear,
        string title,
        bool isLocked,
        DateTime createDate)
    {
        this.SchoolYear = schoolYear;
        this.Title = title;
        this.IsLocked = isLocked;
        this.CreateDate = createDate;
        this.LastMessageDate = createDate;
    }

    public int SchoolYear { get; private set; }
    public int ConversationId { get; private set; }
    public string? ParticipantsInfo { get; private set; }
    public string Title { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int? LastMessageId { get; private set; }
    public DateTime LastMessageDate { get; private set; }
    public bool IsLocked { get; private set; }
    public byte[] Version { get; } = null!;

    // relations
    private readonly List<ConversationParticipant> participants = new();
    public IReadOnlyCollection<ConversationParticipant> Participants => this.participants.AsReadOnly();

    private readonly List<ConversationMessage> messages = new();
    public IReadOnlyCollection<ConversationMessage> Messages => this.messages.AsReadOnly();

    public void AddParticipants(ConversationParticipant[] conversationParticipants)
    {
        this.participants.AddRange(conversationParticipants);
    }

    public void UpdateParticipantsInfo(string participantsInfo)
    {
        this.ParticipantsInfo = participantsInfo;
    }

    public void UpdateParticipantLastReadMessage(int sysUserId, int messageId, DateTime messageDate)
    {
        foreach (var conversationParticipant in this.participants.Where(p => p.SysUserId == sysUserId))
        {
            conversationParticipant.UpdateLastReadMessage(messageId, messageDate);
        }
    }

    public void UpdateLastReadMessage(int messageId, DateTime messageCreateDate)
    {
        this.LastMessageId = messageId;
        this.LastMessageDate = messageCreateDate;
    }
}
