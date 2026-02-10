namespace Neispuo.Tools.Services.Implementations
{
    using Neispuo.Tools.DataAccess;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Neispuo.Tools.Services.Interfaces;

    public class MessageService: BaseService, IMessageService
    {
        public MessageService(NeispuoContext context,
            ILogger<MessageService> logger
)
            : base(context, logger)
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
