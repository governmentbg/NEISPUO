using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MON.Models.Identity;
using MON.Services.Interfaces;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    [Authorize]
    public class AuthenticationController : BaseApiController
    {
        private readonly INeispuoAuthenticationService _authenticationService;


        public AuthenticationController(INeispuoAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<UserInfoViewModel>> GetUserInfo()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            var model = await _authenticationService.GetUserInfo();
            return model;
        }
    }
}
