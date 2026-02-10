namespace SB.ApiAbstractions;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            // add the security headers recommended for backend apis by Mozilla
            // https://observatory.mozilla.org/faq/
            // the HSTS header is not added as SSL is offloaded by the loadbalancer
            context.Response.Headers.TryAdd("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");
            context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
            return Task.CompletedTask;
        });

        await this.next(context);
    }
}
