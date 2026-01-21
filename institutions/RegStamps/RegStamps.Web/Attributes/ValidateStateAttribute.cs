namespace RegStamps.Web.Attributes
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Models.Constants;

    public class ValidateStateAttribute : ActionFilterAttribute
    {
        private readonly string requestField;

        public ValidateStateAttribute(string requestField)
        {
            this.requestField = requestField;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            dynamic requestParameter;

            var isExistParameter = context.ActionArguments
                .TryGetValue(this.requestField, out requestParameter);


            if (isExistParameter)
            {
                if (string.IsNullOrWhiteSpace(requestParameter))
                {
                    context.Result = new JsonResult(JsonStatus.Fail);
                }
            }
            else
            {
                context.Result = new JsonResult(JsonStatus.Fail);
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
