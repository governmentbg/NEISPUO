namespace MON.Services.Security.Permissions
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public sealed class ClassPermissionLevelGroup
    {
        // <gropName, permissions> Права за дадена група.
        private static Dictionary<int, HashSet<string>> _groupsPermissions;

        public ClassPermissionLevelGroup()
        {
            _groupsPermissions = new Dictionary<int, HashSet<string>>
            {
                {
                    (int)PermissionGroupEnum.Reader,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForClassStudentsRead,
                        DefaultPermissions.PermissionNameForClassAbsenceRead,
                        DefaultPermissions.PermissionNameForOresRead,
                    }
                },
                {
                    (int)PermissionGroupEnum.Owner,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForClassManage,
                        DefaultPermissions.PermissionNameForClassStudentsRead,
                        DefaultPermissions.PermissionNameForClassAbsenceRead,
                        DefaultPermissions.PermissionNameForClassAbsenceManage,
                        DefaultPermissions.PermissionNameForStudentToClassEnrollment,
                        DefaultPermissions.PermissionNameForLodStateManage,
                        DefaultPermissions.PermissionNameForOresRead,
                        DefaultPermissions.PermissionNameForOresManage,
                    }
                },
                {
                    (int)PermissionGroupEnum.LeadTeacher,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForClassManage,
                        DefaultPermissions.PermissionNameForClassStudentsRead,
                        DefaultPermissions.PermissionNameForClassAbsenceManage,
                        DefaultPermissions.PermissionNameForOresRead,
                        DefaultPermissions.PermissionNameForLodStateManage
                    }
                },
                {
                    (int)PermissionGroupEnum.LodReader,
                    new HashSet<string>()
                },
                {
                    (int)PermissionGroupEnum.DiplomaSpecial,
                    new HashSet<string>()
                },
                {
                    (int)PermissionGroupEnum.MassEntrollmentManager,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentClassMassEnrolmentManage,
                    }
                }
            };
        }

        private static readonly Lazy<ClassPermissionLevelGroup> Instancelock =
                    new Lazy<ClassPermissionLevelGroup>(() => new ClassPermissionLevelGroup());
        public static ClassPermissionLevelGroup GetInstance
        {
            get
            {
                return Instancelock.Value;
            }
        }

        public Dictionary<int, HashSet<string>> All => _groupsPermissions;
    }
}
