namespace MON.Services.Security.Permissions
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public sealed class InstitutionPermissionLevelGroups
    {
        // <gropName, permissions> Права за дадена група.
        private static Dictionary<int, HashSet<string>> _groupsPermissions;

        public InstitutionPermissionLevelGroups()
        {
            _groupsPermissions = new Dictionary<int, HashSet<string>>
            {
                {
                    (int)PermissionGroupEnum.Reader,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForInstitutionRead,
                        DefaultPermissions.PermissionNameForInstitutionClassesRead,
                        DefaultPermissions.PermissionNameForInstitutionStudentsRead,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForOresRead,
                    }
                },
                {
                    (int)PermissionGroupEnum.Owner,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForInstitutionRead,
                        DefaultPermissions.PermissionNameForInstitutionClassesRead,
                        DefaultPermissions.PermissionNameForInstitutionStudentsRead,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForStudentClassUpdate,
                        DefaultPermissions.PermissionNameForStudentClassHistoryRead,
                        DefaultPermissions.PermissionNameForStudentClassHistoryDelete,
                        DefaultPermissions.PermissionNameForOresRead,
                        DefaultPermissions.PermissionNameForOresManage,
                    }
                },
                {
                    (int)PermissionGroupEnum.LeadTeacher,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForInstitutionRead,
                        DefaultPermissions.PermissionNameForInstitutionClassesRead,
                        DefaultPermissions.PermissionNameForInstitutionStudentsRead,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForOresRead,
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
                    (int)PermissionGroupEnum.AdmissionPermissionRequestManager,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForAdmissionPermissionRequestRead,
                        DefaultPermissions.PermissionNameForAdmissionPermissionRequestManage
                    }
                }
            };
        }

        private static readonly Lazy<InstitutionPermissionLevelGroups> Instancelock =
                    new Lazy<InstitutionPermissionLevelGroups>(() => new InstitutionPermissionLevelGroups());
        public static InstitutionPermissionLevelGroups GetInstance
        {
            get
            {
                return Instancelock.Value;
            }
        }

        public Dictionary<int, HashSet<string>> All => _groupsPermissions;
    }
}
