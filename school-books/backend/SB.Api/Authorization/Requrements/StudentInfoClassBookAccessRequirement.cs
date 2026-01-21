namespace SB.Api;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SB.ApiAbstractions;

public class StudentInfoClassBookAccessRequirement : IAuthorizationRequirement
{
}

public class StudentInfoClassBookAccessRequirementHandler : AuthorizationHandler<StudentInfoClassBookAccessRequirement>
{
    private HttpContext httpContext;
    private IAuthService authService;
    private ILogger<StudentInfoClassBookAccessRequirementHandler> logger;

    public StudentInfoClassBookAccessRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IAuthService authService,
        ILogger<StudentInfoClassBookAccessRequirementHandler> logger)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new Exception($"'{nameof(StudentClassBookAccessRequirementHandler)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
        }

        this.httpContext = httpContextAccessor.HttpContext;
        this.authService = authService;
        this.logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StudentInfoClassBookAccessRequirement requirement)
    {
        OidcToken? token = this.httpContext.GetToken();
        int? schoolYear = this.httpContext.GetIntFromRoute("schoolYear");
        int? instId = this.httpContext.GetIntFromRoute("instId");
        int? classBookId = this.httpContext.GetIntFromRoute("classBookId");
        int? personId = this.httpContext.GetIntFromRoute("personId");

        this.logger.LogInformation("Authorizing StudentClassBookAccessRequirement, {token}, {schoolYear}, {classBookId}, {personId}", token, schoolYear, classBookId, personId);

        if (token != null &&
            schoolYear != null &&
            instId != null &&
            classBookId != null &&
            personId != null &&
            await this.authService.HasStudentInfoClassBookAccessAsync(
                token,
                schoolYear.Value,
                instId.Value,
                classBookId.Value,
                personId.Value,
                this.httpContext.RequestAborted))
        {
            this.logger.LogInformation("Authorization succeeded for StudentClassBookAccessRequirement, {token}, {schoolYear}, {classBookId}, {personId}", token, schoolYear, classBookId, personId);
            context.Succeed(requirement);
            return;
        }

        this.logger.LogInformation("Authorization failed for StudentClassBookAccessRequirement, {token}, {schoolYear}, {classBookId}, {personId}", token, schoolYear, classBookId, personId);
    }
}
