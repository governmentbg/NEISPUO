using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public class MonHRPermissionGroupGenerator : MonPermissionGroupGenerator
    {
        public MonHRPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId)
        {
            PermissionGroups.Add(PermissionGroupEnum.DiplomaSpecial);

            return await Task.FromResult(PermissionGroups);
        }
    }
}
