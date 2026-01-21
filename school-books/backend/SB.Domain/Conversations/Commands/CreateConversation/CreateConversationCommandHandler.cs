namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using static IConversationsQueryRepository;

internal record CreateConversationCommandHandler(IConversationsService conversationsService, INotificationsService notificationsService)
    : IRequestHandler<CreateConversationCommand, CreatedConversationVO>
{
    public async Task<CreatedConversationVO> Handle(CreateConversationCommand command, CancellationToken ct)
    {
        var sysUserId = command.Creator!.SysUserId;

        var conversation = await this.conversationsService.CreateConversationAsync(command, ct);

        await this.notificationsService.TryPostNewMessageNotificationsAsync(conversation.ConversationId, sysUserId, ct);

        return new CreatedConversationVO(conversation.ConversationId, conversation.SchoolYear);
    }
}
