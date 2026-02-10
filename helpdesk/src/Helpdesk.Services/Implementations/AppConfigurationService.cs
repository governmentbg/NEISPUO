namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AppConfigurationService : BaseService, IAppConfigurationService
    {
        public AppConfigurationService(HelpdeskContext context, IUserInfo userInfo, ILogger<AppConfigurationService> logger)
            : base(context, userInfo, logger)
        {

        }
        public Task<string> GetValueByKey(string key)
        {
            return _context.HelpdeskSettings.AsNoTracking()
                .Where(x => x.Key == key)
                .Select(x => x.Value)
                .SingleOrDefaultAsync();
        }
    }
}
