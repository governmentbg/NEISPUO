using MON.Shared.Interfaces;
using System;

namespace MON.Services.Security
{
    public class InstitutionAssociatePermissionGroupGenerator : SchoolPermissionGroupGenerator
    {
        public InstitutionAssociatePermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }
    }
}
