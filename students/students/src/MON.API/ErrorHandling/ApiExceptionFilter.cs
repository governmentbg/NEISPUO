using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MON.Shared.ErrorHandling;
using System;

namespace MON.API.ErrorHandling
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            ApiError apiError = null;
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.Message, ex.ClientNotificationLevel)
                {
                    errors = ex.Errors
                };

                context.HttpContext.Response.StatusCode = ex.StatusCode;

                _logger.LogWarning((ApiException)ex, $"Application thrown error: {ex.Message}");
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access", "error");
                context.HttpContext.Response.StatusCode = 401;
                _logger.LogWarning("Unauthorized Access in Controller Filter.");
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                // Твърде неопределена е грешката
                // var msg = "An unhandled error occurred.";
                var msg = context.Exception.GetBaseException().Message;
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(msg, "error")
                {
                    detail = stack
                };

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
                _logger.LogError(new EventId(0), context.Exception, msg);
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}
