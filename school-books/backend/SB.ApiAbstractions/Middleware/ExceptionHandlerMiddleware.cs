namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

// based upon https://github.com/aspnet/Diagnostics/blob/master/src/Microsoft.AspNetCore.Diagnostics/Internal/DiagnosticsLoggerExtensions.cs
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlerMiddleware> logger;
    private readonly DiagnosticSource diagnosticSource;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        DiagnosticSource diagnosticSource)
    {
        this.next = next;
        this.logger = logger;
        this.diagnosticSource = diagnosticSource;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unhandled exception has occurred while executing the request.");

            if (context.Response.HasStarted)
            {
                this.logger.LogWarning($"The response has already started, the {nameof(ExceptionHandlerMiddleware)} will not handle this exception.");
                throw;
            }

            try
            {
                context.Response.Clear();

                // Preserve the status code that would have been written by the server automatically when a BadHttpRequestException is thrown.
                if (ex is BadHttpRequestException badHttpRequestException)
                {
                    context.Response.StatusCode = badHttpRequestException.StatusCode;
                }
                else
                {
                    context.Response.StatusCode = 500;
                }

                const string eventName = "Microsoft.AspNetCore.Diagnostics.UnhandledException";
                if (this.diagnosticSource.IsEnabled(eventName))
                {
                    this.diagnosticSource.Write(eventName, new { httpContext = context, exception = ex });
                }

                return;
            }
            catch (Exception ex2)
            {
                // If there's a Exception while generating the error page, re-throw the original exception.
                this.logger.LogError(ex2, $"An exception was thrown while attempting to handle an exception by the {nameof(ExceptionHandlerMiddleware)}.");
            }
            throw;
        }
    }
}
