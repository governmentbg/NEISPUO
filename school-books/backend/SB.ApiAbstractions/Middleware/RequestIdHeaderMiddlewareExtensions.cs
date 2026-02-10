namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Builder;

public static class RequestIdHeaderMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestIdHeader(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestIdHeaderMiddleware>();
    }
}
