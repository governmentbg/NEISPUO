namespace Helpdesk.API.Controllers
{
    using Helpdesk.API.ErrorHandling;
    using Helpdesk.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class VersionController : ControllerBase
    {
        [HttpGet]
        public VersionModel Get()
        {
            return new VersionModel()
            {
                Version = Environment.GetEnvironmentVariable("VERSION"),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            };
        }
    }
}
