namespace SB.ExtApi;

using System;
using Microsoft.AspNetCore.Http;
using SB.Domain;

class ExtApiRequestContext : IRequestContext
{
    private HttpContext httpContext;

    public ExtApiRequestContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"'{nameof(ExtApiRequestContext)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");
    }

    public int AuditModuleId => (int)AuditModule.ExtApi;

    public string RequestId => this.httpContext.TraceIdentifier;

    public string RemoteIpAddress =>
        this.httpContext.Connection?.RemoteIpAddress?.ToString()
            ?? throw new Exception($"'{nameof(this.RemoteIpAddress)}' is missing!");

    public bool IsAuthenticated => this.httpContext.User.Identity?.IsAuthenticated ?? false;

    public int? SysUserId => this.httpContext.GetSysUserId();
}
