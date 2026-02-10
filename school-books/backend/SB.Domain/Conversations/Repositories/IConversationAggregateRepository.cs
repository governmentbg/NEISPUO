namespace SB.Domain;

using System.Threading.Tasks;
using System.Threading;

public interface IConversationAggregateRepository : IScopedAggregateRepository<Conversation>
{
    Task<Conversation?> FindByIdOrDefaultAsync(
        int schoolYear,
        int conversationId,
        CancellationToken ct);

    Task<ConversationMessage> AddAsync(ConversationMessage message, CancellationToken ct);
}
