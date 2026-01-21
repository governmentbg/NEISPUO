using Microsoft.EntityFrameworkCore;
using MON.Models;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    public class RuoPermissionGroupGenerator : BasePermissionGroupGenerator
    {
        public RuoPermissionGroupGenerator(IServiceProvider serviceProvider,
            IUserInfo userInfo, PermissionsContextEnum permissionsContext)
            : base(serviceProvider, userInfo, permissionsContext)
        {
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetClassGroupPermissionGroupsForLoggedUser(int classGroupId)
        {
            ClassGroupCacheModel classGroup = await ClassGroupService.GetClassGroupCache(classGroupId);

            if (UserInfo.RegionID.HasValue && UserInfo.RegionID.Value == classGroup?.RegionId)
            {
                PermissionGroups.Add(PermissionGroupEnum.Reader);
            }

            return PermissionGroups;
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetInstitutionPermissionGroupsForLoggedUser(int institutionId)
        {
            InstitutionCacheModel institution = await InstitutionService.GetInstitutionCache(institutionId);

            if (UserInfo.RegionID.HasValue && UserInfo.RegionID.Value == institution?.RegionId)
            {
                PermissionGroups.Add(PermissionGroupEnum.Reader);
            }

            return PermissionGroups;
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId)
        {
            if (!UserInfo.RegionID.HasValue) return PermissionGroups;

            if (await Context.VStudentLists.AnyAsync(x => x.PersonId == studentId
                 && x.RegionId.HasValue && x.RegionId.Value == UserInfo.RegionID.Value))
            {
                PermissionGroups.Add(PermissionGroupEnum.Reader);
                PermissionGroups.Add(PermissionGroupEnum.PersonalDataReader);
                PermissionGroups.Add(PermissionGroupEnum.StudentCurriculumReader);
            }

            PermissionGroups.Add(PermissionGroupEnum.DiplomaSpecial);

            return PermissionGroups;
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
