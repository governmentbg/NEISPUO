using Microsoft.AspNetCore.Mvc.ModelBinding;
using MON.Shared.ErrorHandling;
using System.Linq;

namespace MON.API.ErrorHandling
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
        public string clientNotificationLevel { get; set; }

        public ValidationErrorCollection errors { get; set; }

        public ApiError(string message, string clientNotificationLevel)
        {
            this.message = message;
            this.clientNotificationLevel = clientNotificationLevel;
            isError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            this.isError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                message = "Please correct the specified errors and try again.";
                //errors = modelState.SelectMany(m => m.Value.Errors).ToDictionary(m => m.Key, m=> m.ErrorMessage);
                //errors = modelState.SelectMany(m => m.Value.Errors.Select( me => new KeyValuePair<string,string>( m.Key,me.ErrorMessage) ));
                //errors = modelState.SelectMany(m => m.Value.Errors.Select(me => new ModelError { FieldName = m.Key, ErrorMessage = me.ErrorMessage }));
            }
        }
    }
}
