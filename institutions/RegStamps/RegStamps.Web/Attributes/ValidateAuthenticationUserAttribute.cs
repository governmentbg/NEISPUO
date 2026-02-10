namespace RegStamps.Web.Attributes
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Models.Constants;
    using RegStamps.Web.Controllers;

    public class ValidateAuthenticationUserAttribute : ActionFilterAttribute
    {
        private readonly string[] jsonControllers = { nameof(AutoSaveController), nameof(CheckController) };

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string currentControllerName = $"{context.RouteData.Values.Select(x => x.Value.ToString()).FirstOrDefault()}{nameof(Controller)}";

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = jsonControllers.Contains(currentControllerName)
                    ? new JsonResult(JsonStatus.NoLogin)
                    : new RedirectToRouteResult(new { controller = "Signature", action = "NoCertificate" });
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
