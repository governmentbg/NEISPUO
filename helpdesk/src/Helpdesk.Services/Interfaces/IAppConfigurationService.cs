namespace Helpdesk.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAppConfigurationService
    {
        Task<string> GetValueByKey(string key);
    }
}
