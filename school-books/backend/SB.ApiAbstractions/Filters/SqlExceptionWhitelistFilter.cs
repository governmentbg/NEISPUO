namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SB.Common;
using SB.Domain;

public partial class SqlExceptionWhitelistFilter : IActionFilter
{
    private readonly (KnownSqlErrorType, int)[] sqlExceptionsWhitelist;
    private readonly bool returnSysErrorMessage;

    public SqlExceptionWhitelistFilter((KnownSqlErrorType, int)[] sqlExceptionsWhitelist, bool returnSysErrorMessage)
    {
        this.sqlExceptionsWhitelist = sqlExceptionsWhitelist;
        this.returnSysErrorMessage = returnSysErrorMessage;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is DomainUpdateSqlException ex)
        {
            foreach (var (knownSqlErrorType, statusCode) in this.sqlExceptionsWhitelist)
            {
                if (ex.SqlException.AsKnownSqlError()
                    is KnownSqlError knownSqlError &&
                    knownSqlError.Type == knownSqlErrorType)
                {
                    context.Result =
                        new JsonResult(
                            new ValidationErrorResponse
                            {
                                SysErrorMessage = this.returnSysErrorMessage ? knownSqlError.ErrorMessage : string.Empty
                            })
                        {
                            StatusCode = statusCode,
                        };
                    context.Exception = null;

                    break;
                }
            }
        }
    }
}
