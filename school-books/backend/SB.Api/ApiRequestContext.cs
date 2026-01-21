namespace SB.Api;

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using SB.Domain;

class ApiRequestContext : IRequestContext
{
    private HttpContext httpContext;

    public ApiRequestContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"'{nameof(ApiRequestContext)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
    }

    public int AuditModuleId => (int)AuditModule.Api;

    public string RequestId => this.httpContext.TraceIdentifier;

    public string RemoteIpAddress =>
        this.httpContext.Connection?.RemoteIpAddress?.ToString() ?? throw new Exception($"'{nameof(this.RemoteIpAddress)}' is missing!");

    public string UserAgent => this.httpContext.Request.Headers[HeaderNames.UserAgent].ToString();

    public bool IsAuthenticated => this.httpContext.User?.Identity?.IsAuthenticated ?? false;

    public int? SysUserId => this.httpContext.GetSysUserId();

    public int? SysRoleId => (int?)this.httpContext.GetSysRoleId();

    public string? LoginSessionId => this.httpContext.GetSessionId();

    public string? Username => this.httpContext.GetUsername();

    public string? FirstName => null; // TODO must be added to token

    public string? MiddleName => null; // TODO must be added to token

    public string? LastName => null; // TODO must be added to token
}
