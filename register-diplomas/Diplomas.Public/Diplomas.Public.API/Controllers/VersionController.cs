using Diplomas.Public.API.Infrastructure.ErrorHandling;
using Diplomas.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomas.Public.API.Controllers
{
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
