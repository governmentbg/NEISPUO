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
using static SB.Domain.ITopicPlansQueryRepository;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class TopicPlansController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery] string? name,
        [FromQuery] string? basicClassName,
        [FromQuery] string? subjectName,
        [FromQuery] string? subjectTypeName,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ITopicPlansQueryRepository topicPlansQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    => await topicPlansQueryRepository.GetAllAsync(
        httpContextAccessor.GetSysUserId(),
        name,
        basicClassName,
        subjectName,
        subjectTypeName,
        offset,
        limit,
        ct);

    [HttpGet("{topicPlanId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int topicPlanId,
        [FromServices] ITopicPlansQueryRepository topicPlansQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await topicPlansQueryRepository.GetAsync(
            httpContextAccessor.GetSysUserId(),
            topicPlanId,
            ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateTopicPlanCommand command,
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

    [HttpPost("{topicPlanId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int topicPlanId,
        [FromBody] UpdateTopicPlanCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TopicPlanId = topicPlanId,
            },
            ct);

    [HttpDelete("{topicPlanId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int topicPlanId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveTopicPlanCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TopicPlanId = topicPlanId,
            },
            ct);
}
