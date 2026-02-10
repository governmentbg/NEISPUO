namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Services.Interfaces;
    using System.Threading.Tasks;

    public class TestController : BaseApiController
    {
        private readonly ICacheService _cacheService;


        public TestController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Log()
        {
            Serilog.Log.Fatal("Critical error");
            return Ok();
        }

    }
}
