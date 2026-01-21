using Microsoft.AspNetCore.Authorization;
using MON.Services.Interfaces;
using System.Threading.Tasks;

namespace MON.API.Security
{
    /// <summary>
    /// Handler на Authorize атрибута.
    /// Регистрира се с services.AddScoped<IAuthorizationHandler, PermissionHandler>();
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly INeispuoAuthorizationService _permissionService;
        public PermissionHandler(INeispuoAuthorizationService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail(); // Казва че autorization context-а не е Succeed, независимо от проверките на всчики други IAuthorizationRequirement-и, ако има такива.
                return;
            }

            bool hasPermission = await _permissionService.AuthorizeUser(requirement.Permission.Name);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            // Тук връща 403 при положение, че Authorize атрибута се вика с policy, което е регистрирано с един IAuthorizationRequirement.
            // options.AddPolicy(permission.Name, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
            // Ако има други IAuthorizationRequirement те ще се проверят и може някое от тях да върне context.Succeed(requirement).
        }
    }
}
