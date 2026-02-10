using Diplomas.Public.DataAccess;
using Diplomas.Public.Models.Access;
using Diplomas.Public.Services.Interfaces;
using Diplomas.Public.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diplomas.Public.Services.Implementations
{
    public class AccessService : BaseService, IAccessService
    {
        public AccessService(DiplomasContext context, ILogger<AccessService> logger)
    : base(context, logger)
        {
        }

        public async Task<int> Add(ExtAccessModel model)
        {
            ExtAccess dbAccess = new ExtAccess()
            {
                AuditModuleId = (int)AuditModuleEnum.Diplomas,
                ExtSystemId = model.ExtSystemId,
                ExtSystemServiceId = model.ExtSystemServiceId,
                HasResult = model.HasResult,
                Params = model.Params,
                PersonalIdtype = model.PersonalIdType,
                PersonalId = model.PersonalId,
                Ipaddress = model.IPAddress,
                AccessDate = DateTime.UtcNow
            };

            _context.ExtAccesses.Add(dbAccess);
            await SaveAsync();
            return dbAccess.Id;
        }
    }
}
