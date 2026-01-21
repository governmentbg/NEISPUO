namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using Data;
using static IConversationParticipantsNomRepository;

public partial interface IConversationsQueryRepository
{
    Task<GetUnreadConversationsVO[]> GetUnreadConversationsAsync(
        int sysUserId,
        int? limit,
        CancellationToken ct);

    Task<GetConversationsVO[]> GetConversationsAsync(
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetConversationInfoVO> GetConversationInfoAsync(
        int schoolYear,
        int conversationId,
        int sysUserId,
        CancellationToken ct);

    Task<GetConversationParticipantsVO[]> GetConversationParticipantsAsync(
        int schoolYear,
        int conversationId,
        int sysUserId,
        CancellationToken ct);

    Task<GetConversationMessagesVO[]> GetConversationMessagesAsync(
        int schoolYear,
        int conversationId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<ConversationParticipantsNomVOParticipant[]> GetTeachersParticipantsAsync(
        int instId,
        int? classBookId,
        CancellationToken ct);

    Task<ConversationParticipantsNomVOParticipant[]> GetParentParticipantsAsync(
        int instId,
        int? classBookId,
        CancellationToken ct);

    Task<ConversationParticipantsNomVOParticipant> GetCreatorParticipantInfoAsync(
        int instId,
        int sysUserId,
        ParticipantType participant,
        CancellationToken ct);

    Task<ConversationParticipantsNomVOParticipant[]> GetClassLeadersAsync(
        int instId,
        int[] parentSysUserIds,
        int[] teacherSysUserIds,
        CancellationToken ct);

    Task<bool> CheckIfUserBelongToTheConversation(
        int sysUserId,
        int schoolYear,
        int conversationId,
        CancellationToken ct);
}
