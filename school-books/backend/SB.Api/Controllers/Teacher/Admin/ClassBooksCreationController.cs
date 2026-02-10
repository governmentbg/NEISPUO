namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IClassGroupsQueryRepository;

public class ClassBooksCreationController : AdminController
{
    [HttpGet]
    public async Task<GetAllForClassKindVO[]> GetClassGroupsForClassKindAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] ClassKind classKind,
        [FromServices] IClassGroupsQueryRepository classGroupRepository,
        CancellationToken ct)
        => await classGroupRepository.GetAllForClassKindAsync(schoolYear, instId, classKind, ct);

    [HttpPost]
    public async Task CreateClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateClassBookCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId()
            },
            ct);
}
