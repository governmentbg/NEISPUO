namespace MON.Services.Implementations
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Services.Interfaces;
    using System;
    using System.Threading.Tasks;

    public class UIErrorService : BaseService<UIErrorService>, IUIErrorService
    {
        public UIErrorService(DbServiceDependencies<UIErrorService> dependencies)
            : base(dependencies)
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
                CreatedBySysUserId = _userInfo?.SysUserID,
                CreateDate = DateTime.UtcNow
            };

            _context.Uierrors.Add(uiError);
            await SaveAsync();
            return uiError.Id;
        }
    }
}
