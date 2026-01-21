namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISupportsQueryRepository;

[DisallowWhenInstHasCBExtProvider]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/{classBookId:int}/[action]")]
public class SupportsController
{
    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISupportsQueryRepository supportsQueryRepository,
        CancellationToken ct)
        => await supportsQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpGet("{supportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute][SuppressMessage("", "IDE0060")]int classBookId,
        [FromRoute]int supportId,
        [FromServices]ISupportsQueryRepository supportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        var support = await supportsQueryRepository.GetAsync(schoolYear, supportId, ct);

        support.HasEditAccess = support.HasRemoveAccess =
            await authService.HasSupportAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                supportId,
                ct);

        return support;
    }

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpGet("{supportId:int}/activities")]
    public async Task<ActionResult<TableResultVO<GetActivityAllVO>>> GetActivityAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute][SuppressMessage("", "IDE0060")]int classBookId,
        [FromRoute]int supportId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISupportsQueryRepository supportsQueryRepository,
        CancellationToken ct)
        => await supportsQueryRepository.GetActivityAllAsync(schoolYear, supportId, offset, limit, ct);

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpGet("{supportId:int}/activities/{supportActivityId:int}")]
    public async Task<ActionResult<GetActivityVO>> GetActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute][SuppressMessage("", "IDE0060")]int classBookId,
        [FromRoute]int supportId,
        [FromRoute]int supportActivityId,
        [FromServices]ISupportsQueryRepository supportsQueryRepository,
        CancellationToken ct)
        => await supportsQueryRepository.GetActivityAsync(schoolYear, supportId, supportActivityId, ct);

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost]
    public async Task<int> CreateSupportAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreateSupportCommand command,
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

    [Authorize(Policy = Policies.SupportAccess)]
    [HttpPost("{supportId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int supportId,
        [FromBody]UpdateSupportCommand command,
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
                SupportId = supportId,
            },
            ct);

    [Authorize(Policy = Policies.SupportAccess)]
    [HttpPost("{supportId:int}/activities")]
    public async Task CreateSupportActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int supportId,
        [FromBody]CreateSupportActivityCommand command,
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
                SupportId = supportId,
            },
            ct);

    [Authorize(Policy = Policies.SupportAccess)]
    [HttpPost("{supportId:int}/activities/{supportActivityId:int}")]
    public async Task UpdateActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int supportId,
        [FromRoute]int supportActivityId,
        [FromBody]UpdateSupportActivityCommand command,
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
                SupportId = supportId,
                SupportActivityId = supportActivityId
            },
            ct);

    [Authorize(Policy = Policies.SupportAccess)]
    [HttpDelete("{supportId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int supportId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSupportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SupportId = supportId,
            },
            ct);

    [Authorize(Policy = Policies.SupportAccess)]
    [HttpDelete("{supportId:int}/activities/{supportActivityId:int}")]
    public async Task RemoveActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int supportId,
        [FromRoute]int supportActivityId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSupportActivityCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SupportId = supportId,
                SupportActivityId = supportActivityId
            },
            ct);
}
