namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SB.Domain;

public class DomainExceptionFilter : IActionFilter
{
    private ILogger<DomainExceptionFilter> logger;
    public DomainExceptionFilter(ILogger<DomainExceptionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is DomainValidationException dve)
        {
            context.Result =
                new JsonResult(
                    new ValidationErrorResponse
                    {
                        Errors = dve.Errors,
                        ErrorMessages = dve.ErrorMessages,
                        SysErrorMessage = dve.SysErrorMessage
                    })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            context.Exception = null;

            if (dve.ErrorMessages.Length == 0)
            {
                // the user would not see a human-readable error message
                // we should log this error
                this.logger.LogError(dve, "Non user domain validation exception {@dve}", dve);
            }
        }

        if (context.Exception is DomainObjectNotFoundException)
        {
            context.Result = new ContentResult()
            {
                // its safe to directly send the exception message
                // as we know it is from a DomainObjectNotFoundException
                // and it does not contain sensitive information
                Content = context.Exception.Message,
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status404NotFound,
            };
            context.Exception = null;
        }

        if (context.Exception is DomainUpdateInconsistencyException ||
            context.Exception is DomainUpdateConcurrencyException)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status409Conflict);
            context.Exception = null;
        }
    }
}
