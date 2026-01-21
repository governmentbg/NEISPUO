namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class CurriculumAccessRequirement : IAuthorizationRequirement
{
}

public class CurriculumAccessRequirementHandler : AuthorizationHandler<CurriculumAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public CurriculumAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(CurriculumAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CurriculumAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("schoolYear") is int schoolYear &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            this.httpContext.GetIntFromRoute("classBookId") is int classBookId &&
            this.httpContext.GetIntFromRoute("curriculumId") is int curriculumId &&
            await this.authService.HasCurriculumAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
