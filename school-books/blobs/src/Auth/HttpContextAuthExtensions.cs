namespace SB.Blobs;

using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;

public static class HttpContextAuthExtensions
{
    private const string SelectedRoleClaimType = "selected_role";
    private const string JtiClaimType = "jti";
    private const string ExpClaimType = "exp";
    private const string SessionIdClaimType = "sessionID";

    private static readonly IMemoryCache parsedSelectedRoleCache =
        new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 10000
        });

    public static TokenSelectedRole GetSelectedRole(this HttpContext httpContext)
    {
        var jti = httpContext.User.FindFirstValue(JtiClaimType);
        var exp = httpContext.User.FindFirstValue(ExpClaimType);
        var selectedRoleJSON = httpContext.User.FindFirstValue(SelectedRoleClaimType);

        if (jti == null || exp == null || selectedRoleJSON == null)
        {
            return null;
        }

        return parsedSelectedRoleCache.GetOrCreate(jti, (entry) => {
            long unixTimeSeconds = long.Parse(exp);
            entry.SetAbsoluteExpiration(DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds));
            entry.SetSize(1); // sizing is the number of cached entries

            var selectedRole = JsonSerializer.Deserialize<TokenSelectedRole>(selectedRoleJSON)
                ?? throw new Exception("selectedRoleJSON should not deserialize to null");

            return selectedRole;
        });
    }

    public static string GetSessionId(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(SessionIdClaimType);;
    }
}
