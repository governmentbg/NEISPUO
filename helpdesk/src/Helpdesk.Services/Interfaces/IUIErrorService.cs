namespace Helpdesk.Services.Interfaces
{
    using Helpdesk.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IUIErrorService
    {
        Task<int> Add(ErrorModel model);
    }
}
