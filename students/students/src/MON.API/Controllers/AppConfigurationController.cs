using Microsoft.AspNetCore.Mvc;
using MON.Services.Interfaces;
using System.Threading.Tasks;

namespace MON.API.Controllers
{

    public class AppConfigurationController : BaseApiController
    {
        private readonly IAppConfigurationService _service;

        public AppConfigurationController(IAppConfigurationService service)
        {
            _service = service;
        }

        [HttpGet]

        public Task<string> GetValueByKey(string key)
        {
            return _service.GetValueByKey(key);
        }
    }
}
