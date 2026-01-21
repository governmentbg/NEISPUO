using Diplomas.Public.DataAccess;
using Diplomas.Public.Models;
using Diplomas.Public.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diplomas.Public.Services.Implementations
{
    public class UIErrorService : BaseService, IUIErrorService
    {
        public UIErrorService(DiplomasContext context,
            ILogger<BaseService> logger)
            : base(context, logger)
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
                AdditionalInformation = model.AdditionalInformation,
                CreateDate = DateTime.UtcNow
            };

            await _context.Uierrors.AddAsync(uiError);
            await SaveAsync();
            return uiError.Id;
        }
    }
}
