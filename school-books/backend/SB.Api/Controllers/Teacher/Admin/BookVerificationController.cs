namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IBookVerificationQueryRepository;

public class BookVerificationController : AdminController
{
    [HttpGet]
    public async Task<ActionResult<GetYearViewVO[]>> GetYearViewAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int? classBookId,
        [FromQuery]int? teacherPersonId,
        [FromServices] IBookVerificationQueryRepository bookVerificationQueryRepository,
        CancellationToken ct)
        => await bookVerificationQueryRepository.GetYearViewAsync(
            schoolYear,
            instId,
            classBookId,
            teacherPersonId,
            ct);

    [HttpGet]
    public async Task<ActionResult<GetMonthViewVO[]>> GetMonthViewAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int year,
        [FromQuery]int month,
        [FromQuery]int? classBookId,
        [FromQuery]int? teacherPersonId,
        [FromServices] IBookVerificationQueryRepository bookVerificationQueryRepository,
        CancellationToken ct)
        => await bookVerificationQueryRepository.GetMonthViewAsync(
            schoolYear,
            instId,
            year,
            month,
            classBookId,
            teacherPersonId,
            ct);

    [HttpGet]
    public async Task<ActionResult<GetScheduleLessonsForDayVO[]>> GetScheduleLessonsForDayAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int year,
        [FromQuery]int month,
        [FromQuery]int day,
        [FromQuery]int? classBookId,
        [FromQuery]int? teacherPersonId,
        [FromServices] IBookVerificationQueryRepository bookVerificationQueryRepository,
        CancellationToken ct)
        => await bookVerificationQueryRepository.GetScheduleLessonsForDayAsync(
            schoolYear,
            instId,
            year,
            month,
            day,
            classBookId,
            teacherPersonId,
            ct);

    [HttpPost]
    public async Task UpdateIsVerifiedAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]UpdateIsVerifiedScheduleLessonCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpGet]
    public async Task<ActionResult<GetOffDayVO[]>> GetOffDaysForDayAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int day,
        [FromQuery] int? classBookId,
        [FromServices] IBookVerificationQueryRepository bookVerificationQueryRepository,
        CancellationToken ct)
        => await bookVerificationQueryRepository.GetOffDaysForDayAsync(
            schoolYear,
            instId,
            year,
            month,
            day,
            classBookId,
            ct);
}
