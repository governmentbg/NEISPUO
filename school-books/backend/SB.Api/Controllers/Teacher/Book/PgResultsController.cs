namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IPgResultsQueryRepository;

public class PgResultsController : BookController
{
    [HttpGet]
    public async Task<ActionResult<GetAllForClassBookVO[]>> GetAllForClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] IPgResultsQueryRepository pgResultsQueryRepository,
        CancellationToken ct)
        => await pgResultsQueryRepository.GetAllForClassBookAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllForStudentVO[]>> GetAllForStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int personId,
        [FromServices] IPgResultsQueryRepository pgResultsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var pgResults =
            await pgResultsQueryRepository.GetAllForStudentAsync(
                schoolYear,
                classBookId,
                personId,
                ct);

        foreach (var pgResult in pgResults)
        {
            pgResult.HasEditAccess = pgResult.HasRemoveAccess = hasClassBookAdminWriteAccess;
        }

        return pgResults;
    }

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost]
    public async Task<int> CreatePgResultAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreatePgResultCommand command,
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

    [HttpGet("{pgResultId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int pgResultId,
        [FromServices]IPgResultsQueryRepository pgResultsQueryRepository,
        CancellationToken ct)
        => await pgResultsQueryRepository.GetAsync(schoolYear, classBookId, pgResultId, ct);

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost("{pgResultId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int pgResultId,
        [FromBody]UpdatePgResultCommand command,
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
                PgResultId = pgResultId,
            },
            ct);

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpDelete("{pgResultId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int pgResultId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemovePgResultCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PgResultId = pgResultId,
            },
            ct);
}
