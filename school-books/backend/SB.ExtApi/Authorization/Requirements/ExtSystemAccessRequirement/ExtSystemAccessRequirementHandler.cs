namespace SB.ExtApi;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class ExtSystemAccessRequirementHandler : AuthorizationHandler<ExtSystemAccessRequirement>
{
    private HttpContext httpContext;
    private ExtApiOptions options;
    private ILogger<ExtSystemAccessRequirementHandler> logger;

    public ExtSystemAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IOptions<ExtApiOptions> options,
        ILogger<ExtSystemAccessRequirementHandler> logger)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(ExtSystemAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.options = options.Value;
        this.logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExtSystemAccessRequirement requirement)
    {
        int? extSystemId = this.httpContext.GetExtSystemId();
        int[] extSystemTypes = this.httpContext.GetExtSystemTypes();
        int? sysUserId = this.httpContext.GetSysUserId();

        this.logger.LogInformation("Authorizing ExtSystemAccessRequirement, {extSystemId}, {extSystemTypes}, {sysUserId}", extSystemId, $"[{string.Join(", ", extSystemTypes)}]", sysUserId);

        if (extSystemId.HasValue &&
            requirement.Assertion(new ExtSystemAccessRequirementContext(this.options, extSystemId.Value, extSystemTypes, sysUserId)))
        {
            this.logger.LogInformation("Authorization succeeded for ExtSystemAccessRequirement, {extSystemId}, {extSystemTypes}, {sysUserId}", extSystemId, $"[{string.Join(", ", extSystemTypes)}]", sysUserId);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        this.logger.LogInformation("Authorization failed for ExtSystemAccessRequirement, {extSystemId}, {extSystemTypes}, {sysUserId}", extSystemId, $"[{string.Join(", ", extSystemTypes)}]", sysUserId);
        return Task.CompletedTask;
    }
}
