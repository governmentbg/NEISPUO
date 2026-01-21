namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.ISchedulesQueryRepository;

public class TeacherSchedulesController : AdminController
{
    [HttpGet]
    public async Task<ActionResult<GetTeacherScheduleTableForWeekVO>> GetTeacherScheduleTableForWeekAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int personId,
        [FromQuery] int year,
        [FromQuery] int weekNumber,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
    {
        return await schedulesQueryRepository.GetTeacherScheduleTableForWeekAsync(
            schoolYear,
            instId,
            personId,
            year,
            weekNumber,
            ct);
    }
}
