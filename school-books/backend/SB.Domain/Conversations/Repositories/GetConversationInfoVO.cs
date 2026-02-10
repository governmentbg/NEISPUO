namespace SB.Domain;

using System;

public partial interface IConversationsQueryRepository
{
    public record GetConversationInfoVO(
        int SchoolYear,
        string Title,
        string ParticipantsInfo,
        bool IsLocked,
        bool IsCurrentUserCreator,
        DateTime CreateDate,
        GetConversationParticipantsVO[] Participants);
}
