namespace MON.API.Security
{
    using Microsoft.AspNetCore.Authorization;
    using MON.Services.Security.Permissions;

    /// <summary>
    /// IAuthorizationRequirement който се използва от PermissionHandler(IAuthorizationHandler) middleware-а.
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }

        public Permission Permission { get; }
    }
}
