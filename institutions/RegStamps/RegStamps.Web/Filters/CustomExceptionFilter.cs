namespace RegStamps.Web.Filters
{
    using System;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Controllers;

    using Services.Entities;

    using Models.Constants;

    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly string[] jsonControllers = { nameof(AutoSaveController), nameof(CheckController) };

        private readonly ITbErrorLogService tbErrorLogService;

        public CustomExceptionFilter(ITbErrorLogService tbErrorLogService)
            => this.tbErrorLogService = tbErrorLogService;

        public void OnException(ExceptionContext context)
        {
            int schoolId = context.HttpContext.User.Identity.IsAuthenticated
                ? Convert.ToInt32(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                : -1;

            string urlPath = context.HttpContext.Request.Path;
            string url = $"{context.HttpContext.Request.Host.Value}{urlPath}/{context.HttpContext.Request.QueryString}";

            IEnumerable<string> routeParts = context.RouteData.Values.Select(x => x.Value.ToString());

            string actionName = routeParts.LastOrDefault();
            string controllerName = $"{routeParts.FirstOrDefault()}{nameof(Controller)}";

            Task
                .Run(async () => 
                {
                    await this.tbErrorLogService.CreateLogAsync(schoolId, url, actionName, controllerName, context.Exception);
                })
                .GetAwaiter()
                .GetResult();

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = jsonControllers.Contains(controllerName)
                    ? new JsonResult(JsonStatus.NoLogin)
                    : new RedirectToRouteResult(new { controller = "Signature", action = "NoCertificate" });
            }
            else
            {
                context.Result = jsonControllers.Contains(controllerName)
                    ? new JsonResult(JsonStatus.Fail)
                    : new RedirectToRouteResult(new { controller = "Home", action = "Error" });
            }
        }
    }
}
