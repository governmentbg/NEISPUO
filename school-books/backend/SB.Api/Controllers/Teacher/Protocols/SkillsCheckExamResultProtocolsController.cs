namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISkillsCheckExamResultProtocolsQueryRepository;

public class SkillsCheckExamResultProtocolsController : ProtocolsController
{
    [HttpGet("{skillsCheckExamResultProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromServices] ISkillsCheckExamResultProtocolsQueryRepository skillsCheckExamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await skillsCheckExamResultProtocolsQueryRepository.GetAsync(schoolYear, skillsCheckExamResultProtocolId, ct);

    [HttpGet("{skillsCheckExamResultProtocolId:int}")]
    public async Task<ActionResult<TableResultVO<GetEvaluatorAllVO>>> GetEvaluatorAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ISkillsCheckExamResultProtocolsQueryRepository skillsCheckExamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await skillsCheckExamResultProtocolsQueryRepository.GetEvaluatorAllAsync(schoolYear, skillsCheckExamResultProtocolId, offset, limit, ct);

    [HttpGet("{skillsCheckExamResultProtocolId:int}/{skillsCheckExamResultProtocolEvaluatorId}")]
    public async Task<ActionResult<GetEvaluatorVO>> GetEvaluatorAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromRoute] int skillsCheckExamResultProtocolEvaluatorId,
        [FromServices] ISkillsCheckExamResultProtocolsQueryRepository skillsCheckExamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await skillsCheckExamResultProtocolsQueryRepository.GetEvaluatorAsync(schoolYear, skillsCheckExamResultProtocolId, skillsCheckExamResultProtocolEvaluatorId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateSkillsCheckExamResultProtocolCommand command,
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

    [HttpPost("{skillsCheckExamResultProtocolId:int}")]
    public async Task CreateEvaluatorAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromBody] CreateSkillsCheckExamResultProtocolEvaluatorCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SkillsCheckExamResultProtocolId = skillsCheckExamResultProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{skillsCheckExamResultProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromBody] UpdateSkillsCheckExamResultProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SkillsCheckExamResultProtocolId = skillsCheckExamResultProtocolId,
            },
            ct);

    [HttpPost("{skillsCheckExamResultProtocolId:int}")]
    public async Task UpdateEvaluatorAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromQuery] int skillsCheckExamResultProtocolEvaluatorId,
        [FromBody] UpdateSkillsCheckExamResultProtocolEvaluatorCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SkillsCheckExamResultProtocolId = skillsCheckExamResultProtocolId,
                SkillsCheckExamResultProtocolEvaluatorId = skillsCheckExamResultProtocolEvaluatorId
            },
            ct);

    [HttpDelete("{skillsCheckExamResultProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSkillsCheckExamResultProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SkillsCheckExamResultProtocolId = skillsCheckExamResultProtocolId,
            },
            ct);

    [HttpDelete("{skillsCheckExamResultProtocolId:int}")]
    public async Task RemoveEvaluatorAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamResultProtocolId,
        [FromQuery] int skillsCheckExamResultProtocolEvaluatorId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSkillsCheckExamResultProtocolEvaluatorCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SkillsCheckExamResultProtocolId = skillsCheckExamResultProtocolId,
                SkillsCheckExamResultProtocolEvaluatorId = skillsCheckExamResultProtocolEvaluatorId
            },
            ct);
}
