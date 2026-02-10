namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IPublicationsQueryRepository;

public class PublicationsController : AdminController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        => await publicationsQueryRepository.GetAllAsync(schoolYear, instId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]CreatePublicationCommand command,
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

    [HttpGet("{publicationId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int publicationId,
        [FromServices]IPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        => await publicationsQueryRepository.GetAsync(schoolYear, instId, publicationId, ct);

    [HttpPost("{publicationId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int publicationId,
        [FromBody]UpdatePublicationCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PublicationId = publicationId,
            },
            ct);

    [HttpPost("{publicationId:int}/changeStatus")]
    public async Task ChangeStatusAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int publicationId,
        [FromBody]ChangePublicationStatusCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PublicationId = publicationId,
            },
            ct);

    [HttpDelete("{publicationId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int publicationId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemovePublicationCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PublicationId = publicationId,
            },
            ct);
}
