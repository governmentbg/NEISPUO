namespace Helpdesk.API.Controllers
{
    using Helpdesk.API.ErrorHandling;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class BaseApiController : ControllerBase
    {
        protected ILogger _logger;
        private string GetClaim(string name)
        {
            //First get user claims    
            var claims = User.Claims.ToList();

            //Filter specific claim    
            var claimValue = claims?.FirstOrDefault(x => x.Type.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value;

            return claimValue;
        }

        protected string GetValidationErrors()
        {
            List<string> errors = ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            string errorStr = string.Join("<br />", errors);

            return errorStr;
        }
    }
}
