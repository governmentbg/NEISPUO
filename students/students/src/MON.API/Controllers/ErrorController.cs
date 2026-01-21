namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;
    using MON.Models;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class ErrorController : BaseApiController
    {
        protected readonly IUIErrorService _errorService;

        public ErrorController(ILogger<ErrorController> logger, IUIErrorService errorService)
        {
            _errorService = errorService;
            _logger = logger;
        }

        [HttpPost]
        public Task<int> Add(ErrorModel model)
        {
            model.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            model.UserAgent = Request.Headers[HeaderNames.UserAgent];
            return _errorService.Add(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public Task<int> CspReport([FromBody] CspReportRequest request)
        {
            ErrorModel model = new ErrorModel()
            {
                ModuleId =  (int)AuditModuleEnum.Students,
                Message = JsonConvert.SerializeObject(request),
                Severity = (int)ErrorSeverityEnum.CspError,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = Request.Headers[HeaderNames.UserAgent]
            };

            return _errorService.Add(model);
        }
    }
}
