namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class ClassBookAccessRequirement : IAuthorizationRequirement
{
}

public class ClassBookAccessRequirementHandler : AuthorizationHandler<ClassBookAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public ClassBookAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(ClassBookAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClassBookAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("schoolYear") is int schoolYear &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            this.httpContext.GetIntFromRoute("classBookId") is int classBookId &&
            await this.authService.HasClassBookAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                schoolYear,
                instId,
                classBookId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
