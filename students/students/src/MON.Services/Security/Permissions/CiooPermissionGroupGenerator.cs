using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Security
{


    public class CiooPermissionGroupGenerator : MonPermissionGroupGenerator
    {
        public CiooPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }

        protected override Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId)
        {
            PermissionGroups.Add(PermissionGroupEnum.Reader);
            PermissionGroups.Add(PermissionGroupEnum.PersonalDataReader);
            PermissionGroups.Add(PermissionGroupEnum.StudentCurriculumReader);

            return Task.FromResult(PermissionGroups);
        }
    }
}
