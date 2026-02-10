namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IIndividualWorksQueryRepository;

public class IndividualWorksController : BookController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IIndividualWorksQueryRepository individualWorksQueryRepository,
        CancellationToken ct)
        => await individualWorksQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateIndividualWorkAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreateIndividualWorkCommand command,
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

    [HttpGet("{individualWorkId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int individualWorkId,
        [FromServices]IIndividualWorksQueryRepository individualWorksQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var individualWork = await individualWorksQueryRepository.GetAsync(schoolYear, classBookId, individualWorkId, ct);

        individualWork.HasCreatorAccess = individualWork.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        return individualWork;
    }

    [HttpPost("{individualWorkId:int}")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int individualWorkId,
        [FromBody]UpdateIndividualWorkCommand command,
        [FromServices]IIndividualWorksQueryRepository individualWorksQueryRepository,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        var hasAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var individualWork = !hasAdminWriteAccess ?
            await individualWorksQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                individualWorkId,
                ct) :
            null;

        var hasCreatorAccess =
            individualWork != null &&
            individualWork.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        if (!hasAdminWriteAccess && !hasCreatorAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IndividualWorkId = individualWorkId,
            },
            ct);

        return new OkResult();
    }

    [HttpDelete("{individualWorkId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int individualWorkId,
        [FromServices]IIndividualWorksQueryRepository individualWorksQueryRepository,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        var hasAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var individualWork = !hasAdminWriteAccess ?
            await individualWorksQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                individualWorkId,
                ct) :
            null;

        var hasCreatorAccess =
            individualWork != null &&
            individualWork.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        if (!hasAdminWriteAccess && !hasCreatorAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveIndividualWorkCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IndividualWorkId = individualWorkId,
            },
            ct);

        return new OkResult();
    }
}
