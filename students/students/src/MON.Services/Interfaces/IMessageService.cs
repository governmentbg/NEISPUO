namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public interface IMessageService
    {
        Task<int> SendMessageAsync(MessageModel messageModel);

        Task<IPagedList<MessageViewModel>> ListMy(MessagesListInput input);

        Task<MessageViewModel> GetById(int messageId);

        Task<int> CountMyUnreadMessages();

        Task MarkAsRead(int messageId);

        Task ArchiveAsync(int messageId);

        Task DeleteAsync(int messageId);
        Task DeleteSelected(SelectedEntities model);
        Task ArchiveSelected(SelectedEntities model);
    }
}
