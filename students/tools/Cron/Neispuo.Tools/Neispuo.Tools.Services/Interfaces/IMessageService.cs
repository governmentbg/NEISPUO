namespace Neispuo.Tools.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMessageService
    {
        Task SendMessageAsync(int senderId, int receiverId, string subject, string contents);
    }
}
