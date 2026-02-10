namespace RegStamps.Web.Attributes
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Models.Constants;

    public class ValidateStampRangeAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            dynamic requestParameter;

            var isExistParameter = context.ActionArguments
                .TryGetValue("stampId", out requestParameter);


            if (isExistParameter)
            {
                int stampId = requestParameter;

                if (stampId < 100000 || stampId > 999999)
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
