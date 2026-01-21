namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class HttpContextExtensions
{
    public static string? GetStringFromRoute(this HttpContext httpContext, string key)
    {
        return httpContext.GetRouteValue(key) as string;
    }

    public static int? GetIntFromRoute(this HttpContext httpContext, string key)
    {
        return int.TryParse(httpContext.GetStringFromRoute(key), out int i) ? i : default(int?);
    }
}
