namespace SB.ExtApi;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ServiceFilter(typeof(ClassBookBelongsToInstitutionFilter))]
[ApiController]
[Authorize(Policy = Policies.InstSchoolYearBookAccess)]
[Route("{schoolYear:int}/{institutionId:int}/classBooks/{classBookId:int}/[controller]")]
public abstract class ClassBookSectionController
{
}
