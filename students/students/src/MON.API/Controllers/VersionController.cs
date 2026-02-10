namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.API.ErrorHandling;
    using MON.Models;
    using System;

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
