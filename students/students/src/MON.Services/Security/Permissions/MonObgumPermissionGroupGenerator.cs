using MON.Shared.Interfaces;
using System;

namespace MON.Services.Security
{
    public class MonObgumPermissionGroupGenerator : MonPermissionGroupGenerator
    {
        public MonObgumPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }
    }
}
