using Diplomas.Public.API.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Diplomas.Public.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ExcludeFromCodeCoverage]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    public class BaseApiController : ControllerBase
    {
        protected ILogger _logger;

        public BaseApiController(ILogger logger)
        {
            _logger = logger;
        }
    }
}
