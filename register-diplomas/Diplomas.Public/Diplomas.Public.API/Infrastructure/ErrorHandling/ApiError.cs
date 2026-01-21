using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Diplomas.Public.API.Infrastructure.ErrorHandling
{
    /// <summary>
    /// An API Error response returned to the client
    /// </summary>
    public class ApiError
    {
        public string message { get; set; }
        public bool isError { get; set; }
        public string detail { get; set; }        
        public object data { get; set; }

        public ValidationErrorCollection errors { get; set; }

		public ApiError(string message)
        {
            this.message = message;
            isError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            this.isError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                message = "Please correct the specified errors and try again.";
            }
        }
    }
}
