namespace SB.Api;

using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IClassGroupsQueryRepository;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Http;

public class ClassBooksReorganizeController : AdminController
{
    [HttpGet]
    public async Task<GetAllForClassKindVO[]> GetClassGroupsForClassKindAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] ClassKind classKind,
        [FromQuery] ClassBookReorganizeType reorganizeType,
        [FromServices] IClassGroupsQueryRepository classGroupRepository,
        CancellationToken ct)
        => await classGroupRepository.GetAllForReorganizeAsync(schoolYear, instId, classKind, reorganizeType, ct);

    [HttpPost]
    public async Task CombineClassBooksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CombineClassBooksCommand command,
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

    [HttpPost]
    public async Task SeparateClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] SeparateClassBooksCommand command,
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
