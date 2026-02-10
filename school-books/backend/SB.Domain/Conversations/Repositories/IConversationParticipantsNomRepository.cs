namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IConversationParticipantsNomRepository
{
    Task<ConversationParticipantsNomVO[]> GetNomsByIdAsync(
        int instId,
       ConversationParticipantsNomVOParticipant[] ids,
        CancellationToken ct);

    Task<ConversationParticipantsNomVO[]> GetNomsByTermAsync(
        int instId,
        SysRole userRole,
        int? personId,
        int[]? studentPersonIds,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct);
}
