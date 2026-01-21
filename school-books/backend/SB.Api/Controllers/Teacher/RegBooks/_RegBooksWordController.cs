namespace SB.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = Policies.InstitutionAdminAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]/{recordId:int}/downloadWord")]
public abstract class RegBooksWordController
{
}
