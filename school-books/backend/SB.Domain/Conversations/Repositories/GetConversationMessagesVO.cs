namespace SB.Domain;

using System;

public partial interface IConversationsQueryRepository
{
    public record GetConversationMessagesVO(
        int MessageId,
        string Content,
        int CreatedByParticipantId,
        DateTime CreateDate);
}
