using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MON.Models;
using MON.Models.EduState;
using MON.Services.Implementations;
using MON.Services.Security.Permissions;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MON.Models.Enums;

namespace MON.Services.Security
{
    public class TeacherPermissionGroupGenerator : BasePermissionGroupGenerator
    {
        private readonly EduStateCacheService _eduStateCacheService;

        public TeacherPermissionGroupGenerator(IServiceProvider serviceProvider,
           IUserInfo userInfo, PermissionsContextEnum permissionsContext)
           : base(serviceProvider, userInfo, permissionsContext)
        {
            _eduStateCacheService = serviceProvider?.GetRequiredService<EduStateCacheService>();
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetClassGroupPermissionGroupsForLoggedUser(int classGroupId)
        {
            ClassGroupCacheModel parentClassGroup = await ClassGroupService.GetClassGroupCache(classGroupId);

            if (UserInfo.IsLeadTeacher.HasValue == true
                && UserInfo.LeadTeacherClasses != null
                && (parentClassGroup?.ParentClassId != null
                    && UserInfo.LeadTeacherClasses.Contains(parentClassGroup.ParentClassId.Value))
                    || (parentClassGroup?.ParentClassId == null
                    && UserInfo.LeadTeacherClasses.Contains(classGroupId))
                )

            {
                PermissionGroups.Add(PermissionGroupEnum.LeadTeacher);
            }

            return PermissionGroups;
        }

        protected override Task<HashSet<PermissionGroupEnum>> GetInstitutionPermissionGroupsForLoggedUser(int institutionId)
        {
            if (UserInfo.InstitutionID.HasValue && UserInfo.InstitutionID.Value == institutionId
                && UserInfo.IsLeadTeacher == true)
            {
                PermissionGroups.Add(PermissionGroupEnum.LeadTeacher);
            }

            return Task.FromResult(PermissionGroups);
        }

        protected override async Task<HashSet<PermissionGroupEnum>> GetStudentPermissionGroupsForLoggedUser(int studentId)
        {
            if (UserInfo.InstitutionID.HasValue 
                && UserInfo.IsLeadTeacher == true && UserInfo.LeadTeacherClasses != null && UserInfo.LeadTeacherClasses.Count > 0)
            {
                if (await Context.StudentClasses
                    .AnyAsync(x => x.PersonId == studentId && x.IsCurrent &&
                            (
                                (x.Class.ParentClassId.HasValue && UserInfo.LeadTeacherClasses.Contains(x.Class.ParentClassId.Value))
                                ||
                                (x.Class.ParentClassId == null && UserInfo.LeadTeacherClasses.Contains(x.ClassId))
                            )
                        ))
                {
                    PermissionGroups.Add(PermissionGroupEnum.LeadTeacher);
                }
            }

            return PermissionGroups;
        }

        protected override async Task<HashSet<string>> GetStudentDenyPermissionsForLoggedUser(int studentId)
        {
            HashSet<string> denyPermissions = new HashSet<string>();
            // Липсва InstitutionID На логнатия потребител
            if (!UserInfo.InstitutionID.HasValue) return denyPermissions;

            List<EduStateModel> eduStates = _eduStateCacheService != null
                ? await _eduStateCacheService.GetEduStatesForStudent(studentId)
                : await Context.EducationalStates
                    .AsNoTracking()
                    .Where(x => x.PersonId == studentId && x.PositionId != (int)PositionType.Staff)
                    .Select(x => new EduStateModel
                    {
                        PersonId = x.PersonId,
                        PositionId = x.PositionId,
                        InstitutionId = x.InstitutionId
                    })
                    .ToListAsync();

            int instId = UserInfo.InstitutionID.Value;
            if (eduStates.Any(x => x.InstitutionId == instId && x.PositionId != (int)PositionType.Discharged))
            {
                // Е EduState има запис за даден PersonId и InstitutionId с позиция различна от 9.
                // Следователно е записан в институцията и не следва да има право да създава
                // документи за записване.
                denyPermissions.Add(DefaultPermissions.PermissionNameForStudentAdmissionDocumentCreate);
            }

            if (!eduStates.Any(x => x.InstitutionId == instId && x.PositionId != (int)PositionType.Discharged))
            {
                // Е EduState липса запис за даден PersonId и InstitutionId с позиция различна от 9.
                // Следователно е отписан от институцията и не следва да има право да създава
                // документи за отписване и преместване.
                denyPermissions.Add(DefaultPermissions.PermissionNameForStudentDischargeDocumentCreate);
                denyPermissions.Add(DefaultPermissions.PermissionNameForStudentRelocationDocumentCreate);
            }

            return denyPermissions;
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
