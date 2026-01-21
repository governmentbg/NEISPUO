using Diplomas.Public.Models;
using Diplomas.Public.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomas.Public.API.Controllers
{
    public class ErrorController : BaseApiController
    {
        protected readonly IUIErrorService _errorService;

        public ErrorController(IUIErrorService errorService, ILogger<ErrorController> logger): base(logger)
        {
            _errorService = errorService;
        }

        public Task<int> Add(ErrorModel model)
        {
            model.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            model.UserAgent = Request.Headers[HeaderNames.UserAgent];
            return _errorService.Add(model);
        }
    }
}
