namespace SB.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = Policies.InstitutionAdminAccess)]
[DisallowWhenInstHasCBExtProvider]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public abstract class AdminController
{
}
