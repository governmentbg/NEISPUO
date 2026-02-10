namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IOffDaysQueryRepository;

public class OffDaysController : AdminController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IOffDaysQueryRepository offDaysQueryRepository,
        CancellationToken ct)
        => await offDaysQueryRepository.GetAllAsync(schoolYear, instId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateOffDayAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]CreateOffDayCommand command,
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

    [HttpGet("{offDayId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int offDayId,
        [FromServices]IOffDaysQueryRepository offDaysQueryRepository,
        CancellationToken ct)
        => await offDaysQueryRepository.GetAsync(schoolYear, offDayId, ct);

    [HttpPost("{offDayId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int offDayId,
        [FromBody]UpdateOffDayCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                OffDayId = offDayId,
            },
            ct);

    [HttpDelete("{offDayId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int offDayId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveOffDayCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                OffDayId = offDayId,
            },
            ct);
}
