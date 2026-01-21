namespace SB.Domain;

public partial interface IConversationsQueryRepository
{
    public record GetConversationParticipantsVO(
        int ParticipantId,
        string Title,
        bool IsCreator,
        bool DidReadLastMessage);
}
