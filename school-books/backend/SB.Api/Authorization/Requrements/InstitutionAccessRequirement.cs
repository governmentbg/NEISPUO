namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class InstitutionAccessRequirement : IAuthorizationRequirement
{
}

public class InstitutionAccessRequirementHandler : AuthorizationHandler<InstitutionAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public InstitutionAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(InstitutionAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, InstitutionAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            await this.authService.HasInstitutionAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                instId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
