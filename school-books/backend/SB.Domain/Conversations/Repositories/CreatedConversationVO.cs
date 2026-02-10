namespace SB.Domain;

public partial interface IConversationsQueryRepository
{
    public record CreatedConversationVO(
        int ConversationId, 
        int SchoolYear);
}
