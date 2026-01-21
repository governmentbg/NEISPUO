namespace SB.Domain;

using System;

public partial interface IConversationsQueryRepository
{
    public record GetUnreadConversationsVO(
        int SchoolYear,
        int ConversationId,
        string Title,
        DateTime MessageDate);
}
