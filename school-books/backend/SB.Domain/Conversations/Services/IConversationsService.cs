namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using static IConversationsQueryRepository;

public interface IConversationsService
{
    Task<Conversation> CreateConversationAsync(
        CreateConversationCommand command,
        CancellationToken ct);

    Task<GetConversationMessagesVO[]> GetConversationMessagesAsync(
        int sysUserId,
        int schoolYear,
        int conversationId,
        int? offset,
        int? limit,
        CancellationToken ct);
}
