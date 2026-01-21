using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Security
{
    [ExcludeFromCodeCoverage]
    public class NeispuoAuthorizationService : INeispuoAuthorizationService
    {
        private readonly MONContext _context;
        private readonly IUserInfo _userInfo;
        private readonly SecuritySettings _securitySettings;
        private readonly IServiceProvider _serviceProvider;
        private IInstitutionService _institutionService;

        public NeispuoAuthorizationService(MONContext context,
            IUserInfo userInfo,
            IServiceProvider serviceProvider,
            IOptions<SecuritySettings> securityConfig)
        {
            _context = context;
            _userInfo = userInfo;
            _serviceProvider = serviceProvider;
            _securitySettings = securityConfig?.Value;
        }

        public async Task<bool> AuthorizeUser(string permission)
        {
            int userRole = _userInfo?.SysRoleID ?? int.MinValue;
            InstitutionCacheModel institutionCacheModel = new InstitutionCacheModel();
            HashSet<string> permissonsHash = new HashSet<string>();

            if (_userInfo?.InstitutionID != null)
            {
                institutionCacheModel = await InstitutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);
                foreach (var p in await GetUserPermissionsForInstitution(_userInfo.InstitutionID ?? default))
                {
                    permissonsHash.Add(p);
                }
            }

            HashSet<string> rolePermissions = RolesPermissions.GetRolePermissions(userRole, institutionCacheModel?.InstTypeId);
            if (rolePermissions != null)
            {
                foreach (var p in rolePermissions)
                {
                    permissonsHash.Add(p);
                }
            }

            if (permissonsHash != null)
            {
                bool isGranted = permissonsHash.Contains(permission);
                return isGranted;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AuthorizeUser(string[] permissions, bool hasSomePermission = false)
        {
            int userRole = _userInfo?.SysRoleID ?? int.MinValue;
            InstitutionCacheModel institutionCacheModel = new InstitutionCacheModel();
            HashSet<string> permissonsHash = new HashSet<string>();

            if (_userInfo?.InstitutionID != null)
            {
                institutionCacheModel = await InstitutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);
                foreach (var p in await GetUserPermissionsForInstitution(_userInfo.InstitutionID ?? default))
                {
                    permissonsHash.Add(p);
                }
            }

            HashSet<string> rolePermissions = RolesPermissions.GetRolePermissions(userRole, institutionCacheModel?.InstTypeId);
            if (rolePermissions != null)
            {
                foreach (var p in rolePermissions)
                {
                    permissonsHash.Add(p);
                }
            }

            if (permissonsHash != null)
            {
                bool isGranted = hasSomePermission
                    ? permissions.Any(x => permissonsHash.Contains(x))
                    : permissions.All(x => permissonsHash.Contains(x));
                return isGranted;
            }
            else
            {
                return false;
            }
        }

        protected IInstitutionService InstitutionService
        {
            get
            {
                if (_institutionService == null)
                {
                    _institutionService = _serviceProvider.GetRequiredService<IInstitutionService>();
                }

                return _institutionService;
            }
        }

        public virtual async Task<bool> AuthorizeUser(DemandPermissionModel model)
        {
            if (model == null) return false;

            // Възможност за изключване на проверката чрез конфгурацията на приложението.
            bool hasToDemandPermission = _securitySettings?.DemandPermission ?? true; // true ако не е посочено

            if (!hasToDemandPermission) return true; // Изключили сме проверката за права

            bool hasPermission = await AuthorizeUser(model.Permission); // Проверка за налично право
            if (!hasPermission) return false; // Нямаме търсеното право

            // Имаме търсеното право, но проверяваме контекста.
            return await CheckContextSecurity(model);
        }

        public async Task<bool> HasPermissionForStudent(int personId, string permission)
        {
            HashSet<string> permissonsHash = await GetUserPermissionsForStudent(personId);
            bool isGranted = permissonsHash.Contains(permission ?? "");

            return isGranted;
        }

        public async Task<bool> HasPermissionsForStudent(int personId, string[] permissions)
        {
            HashSet<string> permissonsHash = await GetUserPermissionsForStudent(personId);
            bool isGranted = permissions.All(x => permissonsHash.Contains(x ?? ""));

            return isGranted;
        }

        public async Task<bool> HasPermissionForInstitution(int institutionId, string permission)
        {
            HashSet<string> permissonsHash = await GetUserPermissionsForInstitution(institutionId);
            bool isGranted = permissonsHash.Contains(permission ?? "");

            return isGranted;
        }

        public async Task<bool> HasPermissionForInstitution(int institutionId, string[] permissions)
        {
            HashSet<string> permissonsHash = await GetUserPermissionsForInstitution(institutionId);
            bool isGranted = permissions.All(x => permissonsHash.Contains(x ?? ""));

            return isGranted;
        }

        public async Task<bool> HasPermissionForClass(int classGroupId, string permission)
        {
            HashSet<string> permissonsHash = await GetUserPermissionsForClass(classGroupId);
            bool isGranted = permissonsHash.Contains(permission ?? "");

            return isGranted;
        }

        public async Task<bool> HasPermissionForClass(int classGroupId, string[] permissions)
        {
            HashSet<string> permissonsHash = await GetUserPermissionsForClass(classGroupId);
            bool isGranted = permissions.All(x => permissonsHash.Contains(x ?? ""));

            return isGranted;
        }

        public async Task<HashSet<string>> GetUserPermissions()
        {
            int userRole = _userInfo?.SysRoleID ?? int.MinValue;
            InstitutionCacheModel institutionCacheModel = new InstitutionCacheModel();
            HashSet<string> permissonsHash = new HashSet<string>();

            if (_userInfo?.InstitutionID != null)
            {
                institutionCacheModel = await InstitutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);
                foreach (var p in await GetUserPermissionsForInstitution(_userInfo.InstitutionID ?? default))
                {
                    permissonsHash.Add(p);
                }
            }

            HashSet<string> rolePermissions = RolesPermissions.GetRolePermissions(userRole, institutionCacheModel?.InstTypeId);
            if (rolePermissions != null)
            {
                foreach (var p in rolePermissions)
                {
                    permissonsHash.Add(p);
                }
            }

            return permissonsHash;
        }

        private Task<bool> CheckContextSecurity(DemandPermissionModel model)
        {
            // Възможност за изключване на проверката чрез конфгурацията на приложението.
            if (!(_securitySettings?.DemandPermissionWithContext ?? true)) // true ако не е посочено
            {
                // Изключили сме проверката и връщаме, че сме ауторизирани.
                return Task.FromResult(true);
            }

            if (model.StudentId.HasValue)
            {
                // StudentId има стойност. Проверяваме дали имаме права върху ученика.
                // Todo: Да се мисли дали не може да се кешира.

                // IStudentService го зареждаме през IServiceProvider за да избегнем A circular dependency was detected for the service of type.
                // StudentService инджектва INeispuoAuthorizationService
                IStudentService studentService = _serviceProvider.GetRequiredService<IStudentService>();
                return studentService.CanManageStudent(model.StudentId.Value);
            }

            if (model.InstitutionId.HasValue)
            {
                // InstitutionId има стойност. Проверяваме дали имаме права върху институцията.
                // Todo: Да се мисли дали не може да се кешира.

                // IInstitutionService го зареждаме през IServiceProvider за да избегнем A circular dependency was detected for the service of type.
                // InstitutionService инджектва INeispuoAuthorizationService
                IInstitutionService institutionService = _serviceProvider.GetRequiredService<IInstitutionService>();
                return institutionService.CanManageInstitution(model.InstitutionId.Value);
            }

            if (model.ClassId.HasValue)
            {
                // ClassId има стойност. Проверяваме дали имаме права върху класа през неговата институция.
                // Todo: Да се мисли дали не може да се кешира. 

                // IInstitutionService го зареждаме през IServiceProvider за да избегнем A circular dependency was detected for the service of type.
                // InstitutionService инджектва INeispuoAuthorizationService
                IInstitutionService institutionService = _serviceProvider.GetRequiredService<IInstitutionService>();
                return institutionService.CanManageClass(model.ClassId.Value);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Връща всияки права от <seealso cref="DefaultPermissions"/> като списък от <see cref="Permission"/>
        /// </summary>
        /// <returns></returns>
        public Task<List<Permission>> GetAllPermissons()
        {
            return Task.FromResult(DefaultPermissions.GetAll());
        }

        /// <summary>
        /// Връща всички права върху ученик, определени от връзката между логнатия потребител и ученика (owner, reader и др.)
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task<HashSet<string>> GetUserPermissionsForStudent(int personId)
        {
            return GetUserPermissions(personId, PermissionsContextEnum.Student);
        }

        /// <summary>
        /// Връща всички права върху институция, определени от връзката между логнатия потребител и институцията (owner, reader и др.)
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        public Task<HashSet<string>> GetUserPermissionsForInstitution(int institutionId)
        {
            return GetUserPermissions(institutionId, PermissionsContextEnum.Institution);
        }

        public Task<HashSet<string>> GetUserPermissionsForInstitutionForLoggedUser()
        {
            return GetUserPermissions(_userInfo.InstitutionID, PermissionsContextEnum.Institution);
        }

        /// <summary>
        /// Връща всички права върху група/паралелка, определени от връзката между логнатия потребител и групата/паралелката (owner, reader и др.)
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public Task<HashSet<string>> GetUserPermissionsForClass(int classId)
        {
            return GetUserPermissions(classId, PermissionsContextEnum.ClassGroup);
        }

        private async Task<HashSet<string>> GetUserPermissions(int? entityId, PermissionsContextEnum permissionsContext)
        {
            PermissionGeneratorFactory permissionGeneratorFactory = new PermissionGeneratorFactory(_serviceProvider, _userInfo, permissionsContext);

            HashSet<string> allowPermissions = await permissionGeneratorFactory?.PermissionGenerator?.GetUserAllowPermissions(entityId) ?? new HashSet<string>();
            HashSet<string> denyPermissions = await permissionGeneratorFactory?.PermissionGenerator?.GetUserDenyPermissions(entityId);

            if (denyPermissions != null)
            {
                foreach (var item in denyPermissions)
                {
                    allowPermissions.Remove(item);
                }
            }

            return allowPermissions;
        }
    }
}
