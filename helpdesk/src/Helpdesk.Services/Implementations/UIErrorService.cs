namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UIErrorService : BaseService, IUIErrorService
    {
        public UIErrorService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<BaseService> logger)
            : base(context, userInfo, logger)
        {
        }

        public async Task<int> Add(ErrorModel model)
        {
            var uiError = new Uierror()
            {
                Severity = model.Severity,
                AuditModuleId = model.ModuleId,
                Message = model.Message,
                Trace = model.Trace,
                Ipaddress = model.IpAddress,
                UserAgent = model.UserAgent,
                AdditionalInformation = model.AdditionlInformation,
                CreatedBySysUserId = _userInfo?.SysUserID,
                CreateDate = DateTime.UtcNow
            };

            await _context.Uierrors.AddAsync(uiError);
            await SaveAsync();
            return uiError.Id;
        }
    }
}
