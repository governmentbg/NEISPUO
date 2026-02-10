namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class SupportAccessRequirement : IAuthorizationRequirement
{
}

public class SupportAccessRequirementHandler : AuthorizationHandler<SupportAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public SupportAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(SupportAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SupportAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("schoolYear") is int schoolYear &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            this.httpContext.GetIntFromRoute("classBookId") is int classBookId &&
            this.httpContext.GetIntFromRoute("supportId") is int supportId &&
            await this.authService.HasSupportAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                schoolYear,
                instId,
                classBookId,
                supportId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
