using MON.Shared.Interfaces;
using System;

namespace MON.Services.Security
{
    public class MonObgumFinancePermissionGroupGenerator : MonPermissionGroupGenerator
    {
        public MonObgumFinancePermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }
    }
}
