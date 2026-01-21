namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MessageService: BaseService, IMessageService
    {
        public MessageService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<MessageService> logger
)
            : base(context, userInfo, logger)
        {
        }

        public async Task SendMessageAsync(int senderId, int receiverId, string subject, string contents)
        {
            var message = new Message()
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Subject = subject,
                Contents = contents,
                IsRead = false,
                IsArchived = false,
                IsDeleted = false,
                SendDate = DateTime.Now
            };

            _context.Messages.Add(message);
            await SaveAsync();
        }
    }
}
