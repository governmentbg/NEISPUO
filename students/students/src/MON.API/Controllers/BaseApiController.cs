using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.API.ErrorHandling;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MON.API.Controllers
{
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    [Authorize]
    [Produces("application/json")]
    public class BaseApiController : ControllerBase
    {
        protected ILogger _logger;

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
