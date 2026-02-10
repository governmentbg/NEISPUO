namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class AttendanceDateAccessRequirement : IAuthorizationRequirement
{
}

public class AttendanceDateAccessRequirementHandler : AuthorizationHandler<AttendanceDateAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public AttendanceDateAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(AttendanceDateAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AttendanceDateAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("schoolYear") is int schoolYear &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            this.httpContext.GetIntFromRoute("classBookId") is int classBookId &&
            this.httpContext.GetStringFromRoute("date") is string dateParam &&
            DateTime.TryParse(dateParam, out var date) &&
            await this.authService.HasAttendanceDateAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                schoolYear,
                instId,
                classBookId,
                date,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
