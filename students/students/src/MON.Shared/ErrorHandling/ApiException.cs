using System;
using System.Diagnostics.CodeAnalysis;

namespace MON.Shared.ErrorHandling
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

        public string ClientNotificationLevel { get; set; }

        public ApiException(string message,
                            int statusCode = 500,
                            ValidationErrorCollection errors = null,
                            string clientNotificationLevel = "error"
                            ) :
            base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
            ClientNotificationLevel = clientNotificationLevel;
        }

        public ApiException(string message, Exception ex, int statusCode = 500) : base(message ?? ex.Message, ex)
        {
            StatusCode = statusCode;
            Errors = new ValidationErrorCollection
            {
                new ValidationError(ex.GetInnerMostException()?.Message ?? ex.Message)
            };
        }

        public override string ToString()
        {
            return $"{Errors?.ToString()}  {base.ToString()}";
        }
    }

}
