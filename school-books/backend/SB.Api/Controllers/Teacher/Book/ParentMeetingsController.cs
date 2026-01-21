namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IParentMeetingsQueryRepository;

public class ParentMeetingsController : BookAdminController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IParentMeetingsQueryRepository parentMeetingsQueryRepository,
        CancellationToken ct)
        => await parentMeetingsQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateParentMeetingAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreateParentMeetingCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpGet("{parentMeetingId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int parentMeetingId,
        [FromServices]IParentMeetingsQueryRepository parentMeetingsQueryRepository,
        CancellationToken ct)
        => await parentMeetingsQueryRepository.GetAsync(schoolYear, classBookId, parentMeetingId, ct);

    [HttpPost("{parentMeetingId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int parentMeetingId,
        [FromBody]UpdateParentMeetingCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ParentMeetingId = parentMeetingId,
            },
            ct);

    [HttpDelete("{parentMeetingId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int parentMeetingId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveParentMeetingCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ParentMeetingId = parentMeetingId,
            },
            ct);
}
