using MON.Shared.Interfaces;
using System;

namespace MON.Services.Security
{
    public class MonExpertPermissionGroupGenerator : MonPermissionGroupGenerator
    {
        public MonExpertPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }
    }
}
