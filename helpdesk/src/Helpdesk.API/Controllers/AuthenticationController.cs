namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models.Identity;
    using Helpdesk.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    public class AuthenticationController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;


        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<ActionResult<UserInfoViewModel>> GetUserInfo()
        {
            var model = await _authenticationService.GetUserInfo();
            return model;
        }
    }
}
