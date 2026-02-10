namespace SB.Api;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class DenyAllRequirement : IAuthorizationRequirement
{
}

public class DenyAllRequirementHandler : AuthorizationHandler<DenyAllRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAllRequirement requirement)
    {
        context.Fail();

        return Task.CompletedTask;
    }
}
