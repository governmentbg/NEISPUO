namespace MON.API.Security
{
    using Hangfire.Annotations;
    using Hangfire.Dashboard;

    public class AdministratorHangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
            //HttpContext httpContext = context.GetHttpContext();

            //ClaimsPrincipal user = httpContext?.User;
            //string jsonClaims = user?.FindFirstValue("selected_role");

            //if (jsonClaims == null) return false;

            //UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonClaims);


            //return userInfo?.IsConsortium ?? false;
        }
    }
}