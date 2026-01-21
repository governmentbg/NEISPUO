using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public class RuoExpertPermissionGroupGenerator : RuoPermissionGroupGenerator
    {
        public RuoExpertPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }
    }
}
