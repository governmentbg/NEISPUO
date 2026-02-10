namespace SB.Api;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

public static class HttpContextAuthExtensions
{
    private const string SelectedRoleClaimType = "selected_role";
    private const string JtiClaimType = "jti";
    private const string ExpClaimType = "exp";
    private const string SessionIdClaimType = "sessionID";

    private static readonly IMemoryCache parsedSelectedRoleCache =
        new MemoryCache(new MemoryCacheOptions
        {
            // 50 MB size limit in bytes
            SizeLimit = 50 * 1024 * 1024,

            // Compact the cache by 30% when the size limit is exceeded. We are increasing this
            // from the default value of 5% to prevent the cache from compacting too frequently
            CompactionPercentage = 0.3
        });

    private static readonly ConcurrentDictionary<string, AccessType?> actionDescriptorOverriddenAccessType = new();

    public static OidcToken? GetToken(this HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var jti = httpContext.User.FindFirstValue(JtiClaimType) ?? throw new Exception("Missing jti claim.");
        var exp = httpContext.User.FindFirstValue(ExpClaimType) ?? throw new Exception("Missing exp claim.");
        var sessionId = httpContext.User.FindFirstValue(SessionIdClaimType) ?? throw new Exception("Missing sessionId claim.");
        var selectedRoleJSON = httpContext.User.FindFirstValue(SelectedRoleClaimType) ?? throw new Exception("Missing selectedRole claim.");

        var selectedRole = parsedSelectedRoleCache.GetOrCreate(jti, (entry) => {
            long unixTimeSeconds = long.Parse(exp);
            entry.SetAbsoluteExpiration(DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds));
            entry.SetSize(500); // roughly the size in bytes for each entry in the cache

            var selectedRole = JsonSerializer.Deserialize<OidcTokenSelectedRole>(selectedRoleJSON)
                ?? throw new Exception("selectedRoleJSON should not deserialize to null");

            return selectedRole;
        }) ?? throw new Exception("selectedRole should never be null");

        return new OidcToken(
            jti,
            exp,
            sessionId,
            selectedRole);
    }

    public static int? GetSysUserId(this HttpContext httpContext)
    {
        return httpContext.GetToken()?.SelectedRole?.SysUserId;
    }

    public static int? GetSysRoleId(this HttpContext httpContext)
    {
        return httpContext.GetToken()?.SelectedRole?.SysUserId;
    }

    public static string? GetSessionId(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(SessionIdClaimType);
    }

    public static string? GetUsername(this HttpContext httpContext)
    {
        return httpContext.GetToken()?.SelectedRole?.Username;
    }

    public static AccessType GetAccessType(this HttpContext httpContext)
    {
        AccessType? overriddenAccessType = null;

        var endpoint = httpContext.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (actionDescriptor != null)
        {
            overriddenAccessType = actionDescriptorOverriddenAccessType.GetOrAdd(
                actionDescriptor.Id,
                (_) =>
                {
                    var customActionAttributes = actionDescriptor?.MethodInfo?.GetCustomAttributes(false);

                    if (customActionAttributes?.FirstOrDefault(a => a is OverrideAccessTypeAttribute)
                        is OverrideAccessTypeAttribute overrideAccessTypeAttribute)
                    {
                        return overrideAccessTypeAttribute.AccessType;
                    }

                    return null;
                });
        }

        return overriddenAccessType ??
            (HttpMethods.IsGet(httpContext.Request.Method)
                ? AccessType.Read
                : AccessType.Write);
    }

    public static OidcToken GetToken(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(GetToken)}' called outside of a request.");

        var token = httpContext.GetToken() ??
            throw new Exception("No token! User may be unauthenticated.");

        return token;
    }

    public static int GetSysUserId(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(GetSysUserId)}' called outside of a request.");

        var selectedRole = httpContext.GetToken()?.SelectedRole ??
            throw new Exception("No selected role! User may be unauthenticated.");

        return selectedRole.SysUserId;
    }

    public static int? GetPersonId(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(GetPersonId)}' called outside of a request.");

        var selectedRole = httpContext.GetToken()?.SelectedRole ??
            throw new Exception("No selected role! User may be unauthenticated.");

        return selectedRole.PersonId;
    }

    public static int[] GetStudentPersonIds(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(GetStudentPersonIds)}' called outside of a request.");

        var selectedRole = httpContext.GetToken()?.SelectedRole;

        if (selectedRole?.SysRoleId == SysRole.Student && selectedRole?.PersonId != null)
        {
            return new[] { selectedRole.PersonId.Value };
        }
        else if (selectedRole?.SysRoleId == SysRole.Parent && selectedRole?.StudentPersonIds != null)
        {
            return selectedRole.StudentPersonIds;
        }
        else
        {
            throw new Exception("No student personIds! User may be unauthenticated or is not student or parent.");
        }
    }

    public static int? GetInstId(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(GetInstId)}' called outside of a request.");

        var selectedRole = httpContext.GetToken()?.SelectedRole ??
            throw new Exception("No selected role! User may be unauthenticated.");

        return selectedRole.InstitutionId;
    }

    public static async Task<bool> HasInstitutionAdminReadAccessAsync(
        this IHttpContextAccessor httpContextAccessor,
        int instId,
        CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(HasInstitutionAdminReadAccessAsync)}' called outside of a request.");

        var token = httpContext.GetToken() ??
            throw new Exception("No token! User may be unauthenticated.");

        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();

        return await authService.HasInstitutionAdminAccessAsync(
            token,
            AccessType.Read,
            instId,
            ct);
    }

    public static async Task<bool> HasInstitutionAdminWriteAccessAsync(
        this IHttpContextAccessor httpContextAccessor,
        int instId,
        CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(HasInstitutionAdminWriteAccessAsync)}' called outside of a request.");

        var token = httpContext.GetToken() ??
            throw new Exception("No token! User may be unauthenticated.");

        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();

        return await authService.HasInstitutionAdminAccessAsync(
            token,
            AccessType.Write,
            instId,
            ct);
    }

    public static async Task<bool> HasClassBookWriteAccessAsync(
        this IHttpContextAccessor httpContextAccessor,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(HasClassBookWriteAccessAsync)}' called outside of a request.");

        var token = httpContext.GetToken() ??
            throw new Exception("No token! User may be unauthenticated.");

        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();

        return await authService.HasClassBookAccessAsync(
            token,
            AccessType.Write,
            schoolYear,
            instId,
            classBookId,
            ct);
    }

    public static async Task<bool> HasClassBookAdminWriteAccessAsync(
        this IHttpContextAccessor httpContextAccessor,
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception($"Method '{nameof(HasClassBookAdminWriteAccessAsync)}' called outside of a request.");

        var token = httpContext.GetToken() ??
            throw new Exception("No token! User may be unauthenticated.");

        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();

        return await authService.HasClassBookAdminAccessAsync(
            token,
            AccessType.Write,
            schoolYear,
            instId,
            classBookId,
            ct);
    }
}
