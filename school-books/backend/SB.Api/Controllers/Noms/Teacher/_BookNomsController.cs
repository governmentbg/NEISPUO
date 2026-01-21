namespace SB.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = Policies.ClassBookAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/{classBookId:int}/[action]")]
public abstract class BookNomsController
{
}
