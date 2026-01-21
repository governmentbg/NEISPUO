namespace RegStamps.Web.Attributes
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Controllers;

    using Infrastructure.Extensions;

    using Services.Validation;

    using Models.Constants;

    public class ValidateActiveRequestAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            dynamic requestParameter;

            var isExistParameter = context.ActionArguments
                .TryGetValue("param", out requestParameter);


            if (isExistParameter)
            {
                if (string.IsNullOrWhiteSpace(requestParameter))
                {
                    context.Result = this.CreateInstance();
                }

                int stampId = $"{requestParameter}".FromBase64ConvertToInt();
                int schoolId = Convert.ToInt32(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                BaseController baseController = context.Controller as BaseController;

                IValidationService validationService = context
                                                        .HttpContext
                                                        .RequestServices?
                                                        .GetService(typeof(IValidationService)) as IValidationService;

                if (validationService is null)
                {
                    baseController.TempData[GlobalConstants.Error] = "Възникна грешка";
                    context.Result = this.CreateInstance();
                }

                (bool isValid, string message) = await validationService.ValidateActiveRequestExistAsync(schoolId, stampId);

                if (!isValid)
                {
                    baseController.TempData[GlobalConstants.Warning] = message;

                    context.Result = this.CreateInstance();
                }
            }
            else
            {
                context.Result = this.CreateInstance();
            }

            await base.OnActionExecutionAsync(context, next);
        }

        private RedirectToRouteResult CreateInstance()
             => new RedirectToRouteResult(new { controller = "Stamp", action = "ListStamp" });
    }
}
