namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MessageController : BaseApiController
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<int> SendMessage(MessageModel input)
        {
            return await _service.SendMessageAsync(input);
        }

        [HttpGet]
        public async Task<MessageViewModel> GetById(int messageId)
        {
            return await _service.GetById(messageId);
        }

        [HttpGet]
        public async Task<IPagedList<MessageViewModel>> ListMy([FromQuery] MessagesListInput input)
        {
            return await _service.ListMy(input);
        }

        [HttpGet]
        public async Task<int> CountMyUnreadMessages()
        {
            return await _service.CountMyUnreadMessages();
        }

        [HttpPut]
        public async Task MarkAsRead(int messageId)
        {
            await _service.MarkAsRead(messageId);
        }


        [HttpPut]
        public async Task ArchiveMessage(int messageId)
        {
            await _service.ArchiveAsync(messageId);
        }


        [HttpDelete]
        public async Task DeleteMessage(int messageId)
        {
            await _service.DeleteAsync(messageId);
        }

        [HttpPost]
        public async Task DeleteSelected(SelectedEntities model)
        {
            await _service.DeleteSelected(model);
        }

        [HttpPost]
        public async Task ArchiveSelected(SelectedEntities model)
        {
            await _service.ArchiveSelected(model);
        }
    }
}
