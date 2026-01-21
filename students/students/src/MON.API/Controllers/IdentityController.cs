using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MON.API.Controllers
{
    [Authorize]
    public class IdentityController : BaseApiController
    {
        [HttpGet]
        public IActionResult Claims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminClaims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
