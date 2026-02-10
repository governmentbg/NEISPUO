using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public class MonPermissionGroupGenerator : BasePermissionGroupGenerator
    {
        public MonPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }

        protected override Task<HashSet<PermissionGroupEnum>> GetClassGroupPermissionGroupsForLoggedUser(int classGroupId)
        {
            PermissionGroups.Add(PermissionGroupEnum.Reader);

            return Task.FromResult(PermissionGroups);
        }

        protected override Task<HashSet<PermissionGroupEnum>> GetInstitutionPermissionGroupsForLoggedUser(int institutionId)
        {
            PermissionGroups.Add(PermissionGroupEnum.Reader);

            return Task.FromResult(PermissionGroups);
        }

        protected override Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId)
        {
            PermissionGroups.Add(PermissionGroupEnum.Reader);
            PermissionGroups.Add(PermissionGroupEnum.PersonalDataReader);

            return Task.FromResult(PermissionGroups);
        }

        protected override Task<HashSet<string>> GetStudentDenyPermissionsForLoggedUser(int studentId)
        {
            return Task.FromResult(new HashSet<string>());
        }

        protected override Task<HashSet<string>> GetInstitutionDenyPermissionsForLoggedUser(int institutionId)
        {
            return Task.FromResult(new HashSet<string>());
        }
        protected override Task<HashSet<string>> GetClassDenyPermissionsForLoggedUser(int classGroupId)
        {
            return Task.FromResult(new HashSet<string>());
        }
    }
}
