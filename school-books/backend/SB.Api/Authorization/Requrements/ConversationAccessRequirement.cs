namespace SB.Api;

using System;
using System.Threading.Tasks;
using ApiAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

public class ConversationAccessRequirement : IAuthorizationRequirement
{
}

public class ConversationAccessRequirementHandler : AuthorizationHandler<ConversationAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;

    public ConversationAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(ConversationAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ConversationAccessRequirement requirement)
    {
        if (this.httpContext.GetToken() is OidcToken token &&
            this.httpContext.GetIntFromRoute("instId") is int instId &&
            await this.authService.HasConversationsAccessAsync(
                token,
                this.httpContext.GetAccessType(),
                instId,
                this.httpContext.RequestAborted))
        {
            context.Succeed(requirement);
        }
    }
}
