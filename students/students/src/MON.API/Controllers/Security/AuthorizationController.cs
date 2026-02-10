using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MON.Models;
using MON.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    [Authorize]
    public class AuthorizationController : BaseApiController
    {
        private readonly INeispuoAuthorizationService _authorizationService;
        public AuthorizationController(INeispuoAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<HashSet<string>> GetUserPermissions()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HashSet<string>();
            }

            return await _authorizationService.GetUserPermissions();
        }

        [HttpGet]
        public async Task<HashSet<string>> GetUserPermissionsForStudent(int personId)
        {
            return await _authorizationService.GetUserPermissionsForStudent(personId);
        }

        [HttpGet]
        public async Task<HashSet<string>> GetUserPermissionsForInstitution(int institutionId)
        {
            return await _authorizationService.GetUserPermissionsForInstitution(institutionId);
        }

        [HttpGet]
        public async Task<HashSet<string>> GetUserPermissionsForInstitutionForLoggedUser()
        {
            return await _authorizationService.GetUserPermissionsForInstitutionForLoggedUser();
        }

        [HttpGet]
        public async Task<HashSet<string>> GetUserPermissionsForClass(int classId)
        {
            return await _authorizationService.GetUserPermissionsForClass(classId);
        }

        [HttpPost]
        public async Task<bool> Authorize(DemandPermissionModel model)
        {
            return await _authorizationService.AuthorizeUser(model);
        }
    }
}
