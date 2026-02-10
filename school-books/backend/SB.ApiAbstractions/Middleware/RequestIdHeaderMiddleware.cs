namespace SB.ApiAbstractions;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class RequestIdHeaderMiddleware
{
    public static readonly string RequestIdHeaderName = "X-Sb-Request-Id";

    private readonly RequestDelegate next;

    public RequestIdHeaderMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(RequestIdHeaderName, context.TraceIdentifier);
            return Task.CompletedTask;
        });

        await this.next(context);
    }
}
