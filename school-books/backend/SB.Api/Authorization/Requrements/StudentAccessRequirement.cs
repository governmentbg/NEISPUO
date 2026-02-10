namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

public class StudentAccessRequirement : IAuthorizationRequirement
{
}

public class StudentAccessRequirementHandler : AuthorizationHandler<StudentAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public StudentAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(StudentClassBookAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StudentAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            await this.authService.HasStudentAccessAsync(
                token,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
