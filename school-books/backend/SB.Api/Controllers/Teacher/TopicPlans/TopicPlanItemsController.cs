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
public class TopicPlanItemsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery] int topicPlanId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ITopicPlansQueryRepository topicPlansQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await topicPlansQueryRepository.GetItemsAsync(
            httpContextAccessor.GetSysUserId(),
            topicPlanId,
            offset,
            limit,
            ct);

    [HttpGet("{topicPlanItemId:int}")]
    public async Task<ActionResult<GetItemVO>> GetItemAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int topicPlanItemId,
        [FromServices] ITopicPlansQueryRepository topicPlansQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await topicPlansQueryRepository.GetItemAsync(
            httpContextAccessor.GetSysUserId(),
            topicPlanItemId,
            ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateTopicPlanItemCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{topicPlanItemId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int topicPlanItemId,
        [FromBody] UpdateTopicPlanItemCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TopicPlanItemId = topicPlanItemId,
            },
            ct);

    [HttpDelete("{topicPlanItemId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int topicPlanItemId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveTopicPlanItemCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TopicPlanItemId = topicPlanItemId,
            },
            ct);

    [HttpDelete("{topicPlanId:int}")]
    public async Task RemoveAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int topicPlanId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveAllTopicPlanItemsCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TopicPlanId = topicPlanId,
            },
            ct);
}
