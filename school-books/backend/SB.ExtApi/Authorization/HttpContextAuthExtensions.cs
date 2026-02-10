namespace SB.ExtApi;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public static class HttpContextAuthExtensions
{
    public static int? GetExtSystemId(this IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.GetExtSystemId()
            ?? throw new Exception($"'{nameof(HttpContextAuthExtensions.GetExtSystemId)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");

    public static int[] GetExtSystemTypes(this IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.GetExtSystemTypes()
            ?? throw new Exception($"'{nameof(HttpContextAuthExtensions.GetExtSystemTypes)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");

    public static int? GetSysUserId(this IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.GetSysUserId()
            ?? throw new Exception($"'{nameof(HttpContextAuthExtensions.GetSysUserId)}' called outside of a request. IHttpContextAccessor.HttpContext is null!");

    public static int? GetExtSystemId(this HttpContext httpContext)
    {
        if (httpContext.User.Identity is ClaimsIdentity identity)
        {
            return identity.GetExtSystemId();
        }

        return null;
    }

    public static int[] GetExtSystemTypes(this HttpContext httpContext)
    {
        if (httpContext.User.Identity is ClaimsIdentity identity)
        {
            return identity.GetExtSystemTypes();
        }

        return Array.Empty<int>();
    }

    public static int? GetSysUserId(this HttpContext httpContext)
    {
        if (httpContext.User.Identity is ClaimsIdentity identity)
        {
            return identity.GetSysUserId();
        }

        return null;
    }

    public static int? GetExtSystemId(this ClaimsIdentity identity)
    {
        if (identity.IsAuthenticated &&
            identity.FindFirst(ExtSystemClaimsPrincipalProvider.ExtSystemIdClaimType) is Claim { Value: string extSystemId })
        {
            return int.Parse(extSystemId);
        }

        return null;
    }

    public static int[] GetExtSystemTypes(this ClaimsIdentity identity)
    {
        if (identity.IsAuthenticated)
        {
            return identity.FindAll(ExtSystemClaimsPrincipalProvider.ExtSystemTypeClaimType)
                .Select(claim => int.Parse(claim.Value))
                .ToArray();
        }

        return Array.Empty<int>();
    }

    public static int? GetSysUserId(this ClaimsIdentity identity)
    {
        if (identity.IsAuthenticated &&
            identity.FindFirst(ExtSystemClaimsPrincipalProvider.SysUserIdClaimType) is Claim { Value: string sysUserId })
        {
            return int.Parse(sysUserId);
        }

        return null;
    }

    private static readonly ConcurrentDictionary<string, string[]> actionDescriptorAdditionalAccess = new();

    public static string[] GetAdditionalAccess(this HttpContext httpContext)
    {
        string[] additionalAccess = Array.Empty<string>();

        var endpoint = httpContext.GetEndpoint();

        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (actionDescriptor != null)
        {
            additionalAccess = actionDescriptorAdditionalAccess.GetOrAdd(
                actionDescriptor.Id,
                (_) =>
                    (actionDescriptor?.MethodInfo?.GetCustomAttributes(inherit: false) ?? Array.Empty<object>())
                    .Concat(actionDescriptor?.ControllerTypeInfo?.GetCustomAttributes(inherit: false) ?? Array.Empty<object>())
                    .OfType<AdditionalAccessAttribute>()
                    .Select(a => a.Access)
                    .Distinct()
                    .ToArray());
        }

        return additionalAccess;
    }
}
