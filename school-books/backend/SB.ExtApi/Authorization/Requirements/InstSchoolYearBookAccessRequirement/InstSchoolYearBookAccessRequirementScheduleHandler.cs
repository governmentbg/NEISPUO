namespace SB.ExtApi;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SB.ApiAbstractions;
using SB.Domain;

public class InstSchoolYearBookAccessRequirementScheduleHandler : AuthorizationHandler<InstSchoolYearBookAccessRequirement>
{
    private HttpContext httpContext;
    private ICommonCachedQueryStore commonCachedQueryStore;
    private ILogger<InstSchoolYearBookAccessRequirementScheduleHandler> logger;

    public InstSchoolYearBookAccessRequirementScheduleHandler(
        IHttpContextAccessor httpContextAccessor,
        ICommonCachedQueryStore commonCachedQueryStore,
        ILogger<InstSchoolYearBookAccessRequirementScheduleHandler> logger)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(InstSchoolYearBookAccessRequirementScheduleHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.commonCachedQueryStore = commonCachedQueryStore;
        this.logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, InstSchoolYearBookAccessRequirement requirement)
    {
        int? extSystemId = this.httpContext.GetExtSystemId();
        int[] extSystemTypes = this.httpContext.GetExtSystemTypes();
        int? sysUserId = this.httpContext.GetSysUserId();
        int? schoolYear = this.httpContext.GetIntFromRoute("schoolYear");
        int? instId = this.httpContext.GetIntFromRoute("institutionId");
        string[] additionalAccess = this.httpContext.GetAdditionalAccess();

        this.logger.LogInformation("Authorizing InstSchoolYearBookAccessRequirement, {extSystemId}, {extSystemTypes}, {sysUserId}, {schoolYear}, {instId}", extSystemId, $"[{string.Join(", ", extSystemTypes)}]", sysUserId, schoolYear, instId);

        if (extSystemId.HasValue &&
            extSystemTypes.Contains(AuthorizationConstants.ExtSystemTypeSchedule) &&
            sysUserId.HasValue &&
            schoolYear.HasValue &&
            instId.HasValue &&
            additionalAccess.Contains(AuthorizationConstants.ScheduleProviderAdditionalAccess) &&
            (await this.commonCachedQueryStore.GetExtSystemIsInstScheduleExtProviderAsync(
                extSystemId.Value,
                schoolYear.Value,
                instId.Value,
                this.httpContext.RequestAborted)))
        {
            this.logger.LogInformation("Authorization succeeded for InstSchoolYearBookAccessRequirement, {extSystemId}, {extSystemTypes}, {sysUserId}, {schoolYear}, {instId}", extSystemId, $"[{string.Join(", ", extSystemTypes)}]", sysUserId, schoolYear, instId);
            context.Succeed(requirement);
            return;
        }

        this.logger.LogInformation("Authorization failed for InstSchoolYearBookAccessRequirement, {extSystemId}, {extSystemTypes}, {sysUserId}, {schoolYear}, {instId}", extSystemId, $"[{string.Join(", ", extSystemTypes)}]", sysUserId, schoolYear, instId);
    }
}
