namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;

public class HisMedicalNoticeAccessRequirement : IAuthorizationRequirement
{
}

public class HisMedicalNoticeAccessRequirementHandler : AuthorizationHandler<HisMedicalNoticeAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public HisMedicalNoticeAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(HisMedicalNoticeAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HisMedicalNoticeAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("hisMedicalNoticeId") is int hisMedicalNoticeId &&
            await this.authService.HasHisMedicalNoticeAccessAsync(
                token,
                hisMedicalNoticeId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
