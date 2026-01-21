namespace MON.Services.Security
{
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PermissionGeneratorFactory
    {
        internal IPermissionGenerator PermissionGenerator { get; private set; }

        internal PermissionGeneratorFactory(IServiceProvider serviceProvider, IUserInfo userInfo, PermissionsContextEnum permissionsContext)
        {
            PermissionGenerator = userInfo?.UserRole switch
            {
                UserRoleEnum.School => new SchoolPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.Mon => new MonPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.Ruo => new RuoPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.Teacher => new TeacherPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.RuoExpert => new RuoExpertPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.MonExpert => new MonExpertPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.ExternalExpert => new MonExpertPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.Consortium => new ConsortiumPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.CIOO => new CiooPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.InstitutionAssociate => new InstitutionAssociatePermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.MonOBGUM => new MonObgumPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.MonOBGUM_Finance => new MonObgumFinancePermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                UserRoleEnum.MonHR => new MonHRPermissionGroupGenerator(serviceProvider, userInfo, permissionsContext),
                _ => null,
            };
        }
    }

    public enum PermissionsContextEnum
    {
        Student,
        Institution,
        ClassGroup
    }
}
