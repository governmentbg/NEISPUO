namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class ScheduleLessonAccessRequirement : IAuthorizationRequirement
{
}

public class ScheduleLessonAccessRequirementHandler : AuthorizationHandler<ScheduleLessonAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public ScheduleLessonAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(ScheduleLessonAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ScheduleLessonAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("schoolYear") is int schoolYear &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            this.httpContext.GetIntFromRoute("classBookId") is int classBookId &&
            this.httpContext.GetIntFromRoute("scheduleLessonId") is int scheduleLessonId &&
            await this.authService.HasScheduleLessonAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                schoolYear,
                instId,
                classBookId,
                scheduleLessonId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
