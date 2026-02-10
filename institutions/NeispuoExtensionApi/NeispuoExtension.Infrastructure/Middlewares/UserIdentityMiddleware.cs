namespace NeispuoExtension.Infrastructure.Middlewares
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Microsoft.AspNetCore.Http;

    using Services.Core.UserIdentity;

    internal class UserIdentityMiddleware
    {
        private readonly RequestDelegate next;

        public UserIdentityMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IUserIdentityService userIdentityService)
        {
            Claim selectedRole = context.User.Claims
                .FirstOrDefault(c => c.Type == "selected_role");

            if (selectedRole != null)
            {
                var dsd = selectedRole.Value;

                JObject claims = (JObject)JsonConvert
                    .DeserializeObject(selectedRole.Value);
                
                userIdentityService.Username = claims["Username"].ToString();

                int.TryParse(claims["SysUserID"].ToString(), out int sysUserId);
                int.TryParse(claims["SysRoleID"].ToString(), out int sysRoleId);
                int.TryParse(claims["InstitutionID"].ToString(), out int institutionId);

                userIdentityService.SysUserId = sysUserId;
                userIdentityService.SysRoleId = sysRoleId;
                userIdentityService.InstitutionId = institutionId;
            }

            await this.next(context);
        }
    }
}
