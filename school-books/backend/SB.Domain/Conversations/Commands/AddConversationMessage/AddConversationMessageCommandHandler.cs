namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record AddConversationMessageCommandHandler(
    IUnitOfWork UnitOfWork,
    INotificationsService NotificationsService,
    IConversationAggregateRepository ConversationsAggregateRepository)
    : IRequestHandler<AddConversationMessageCommand, int>
{
    public async Task<int> Handle(AddConversationMessageCommand command, CancellationToken ct)
    {
        var schoolYear = command.SchoolYear!.Value;
        var sysUserId = command.SysUserId!.Value;
        var conversationId = command.ConversationId!.Value;

        var conversation = await this.ConversationsAggregateRepository.FindByIdOrDefaultAsync(schoolYear, conversationId, ct);
        var participant = conversation?.Participants.FirstOrDefault(p => p.SysUserId == sysUserId);

        if (conversation == null || participant == null)
        {
            throw new DomainValidationException(
                new[] { "conversation_not_found" },
                new[] { $"Съобщениe с идентификатор {conversationId} не е намерен." });
        }

        var newMessage = new ConversationMessage(
            schoolYear,
            conversation,
            participant.ConversationParticipantId,
            command.Message!,
            DateTime.Now);
        var message = await this.ConversationsAggregateRepository.AddAsync(newMessage, ct);
        conversation.UpdateLastReadMessage(message.ConversationMessageId, message.CreateDate);

        await this.NotificationsService.TryPostNewMessageNotificationsAsync(conversationId, sysUserId, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return message.ConversationId;
    }
}
