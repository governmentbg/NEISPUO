namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models;
    using Helpdesk.Services.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ErrorController : BaseApiController
    {
        protected readonly IUIErrorService _errorService;

        public ErrorController(IUIErrorService errorService)
        {
            _errorService = errorService;
        }

        [HttpPost]
        public Task<int> Add(ErrorModel model)
        {
            model.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            model.UserAgent = Request.Headers[HeaderNames.UserAgent];
            return _errorService.Add(model);
        }
    }
}
