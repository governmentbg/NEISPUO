namespace SB.Domain;

using System;

public partial interface IConversationsQueryRepository
{
    public record GetConversationsVO(
        int SchoolYear,
        int ConversationId,
        string Title,
        bool hasNewMessages,
        DateTime LastMessageDate);
}
