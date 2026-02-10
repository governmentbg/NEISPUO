namespace SB.ExtApi;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize(Policy = Policies.InstSchoolYearBookAccess)]
[Route("{schoolYear:int}/{institutionId:int}/[controller]")]
public abstract class SchoolBookSectionController
{
}
