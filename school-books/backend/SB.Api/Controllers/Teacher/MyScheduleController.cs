namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.ISchedulesQueryRepository;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class MyScheduleController
{
    [HttpGet("{year:int}/{weekNumber:int}")]
    public async Task<ActionResult<GetTeacherScheduleTableForWeekVO>> GetTeacherScheduleTableForWeekAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int year,
        [FromRoute] int weekNumber,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var personId = httpContextAccessor.GetPersonId();

        if (personId == null)
        {
            return new ForbidResult();
        }

        return await schedulesQueryRepository.GetTeacherScheduleTableForWeekAsync(
            schoolYear,
            instId,
            personId.Value,
            year,
            weekNumber,
            ct);
    }
}
