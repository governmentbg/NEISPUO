namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SB.ApiAbstractions;

public class StudentMedicalNoticesAccessRequirement : IAuthorizationRequirement
{
}

public class StudentMedicalNoticesAccessRequirementHandler : AuthorizationHandler<StudentMedicalNoticesAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;
    private ILogger<StudentMedicalNoticesAccessRequirementHandler> logger;

    public StudentMedicalNoticesAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService,
        ILogger<StudentMedicalNoticesAccessRequirementHandler> logger)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(StudentMedicalNoticesAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
        this.logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StudentMedicalNoticesAccessRequirement requirement)
    {
        OidcToken? token = this.httpContext.GetToken();
        int? schoolYear = this.httpContext.GetIntFromRoute("schoolYear");
        int? personId = this.httpContext.GetIntFromRoute("personId");

        this.logger.LogInformation("Authorizing StudentMedicalNoticesAccessRequirement, {token}, {schoolYear}, {personId}", token, schoolYear, personId);

        if (token != null &&
            schoolYear != null &&
            personId != null &&
            await this.authService.HasStudentMedicalNoticesAccessAsync(
                token,
                schoolYear.Value,
                personId.Value,
                this.httpContext.RequestAborted))
        {
            this.logger.LogInformation("Authorization succeeded for StudentMedicalNoticesAccessRequirement, {token}, {schoolYear}, {personId}", token, schoolYear, personId);
            context.Succeed(requirement);
            return;
        }

        this.logger.LogInformation("Authorization succeeded for StudentMedicalNoticesAccessRequirement, {token}, {schoolYear}, {personId}", token, schoolYear, personId);
    }
}
