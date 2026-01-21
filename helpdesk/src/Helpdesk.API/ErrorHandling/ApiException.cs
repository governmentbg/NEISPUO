using System;
using System.Diagnostics.CodeAnalysis;

namespace Helpdesk.API.ErrorHandling
{

    /// <summary>
    /// Custom Exception class that knows about HTTP 
    /// result codes and includes a validation errors
    /// error collection that can optionally be set with
    /// multiple errors.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public ValidationErrorCollection Errors { get; set; }

        public ApiException(string message,
                            int statusCode = 500,
                            ValidationErrorCollection errors = null) :
            base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }

}
