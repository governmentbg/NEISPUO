namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.ISkillsCheckExamDutyProtocolsQueryRepository;

public class SkillsCheckExamDutyProtocolsController : ProtocolsController
{
    [HttpGet("{skillsCheckExamDutyProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int skillsCheckExamDutyProtocolId,
        [FromServices] ISkillsCheckExamDutyProtocolsQueryRepository skillsCheckExamDutyProtocolsQueryRepository,
        CancellationToken ct)
        => await skillsCheckExamDutyProtocolsQueryRepository.GetAsync(schoolYear, skillsCheckExamDutyProtocolId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateSkillsCheckExamDutyProtocolCommand command,
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

    [HttpPost("{skillsCheckExamDutyProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamDutyProtocolId,
        [FromBody] UpdateSkillsCheckExamDutyProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SkillsCheckExamDutyProtocolId = skillsCheckExamDutyProtocolId,
            },
            ct);

    [HttpDelete("{skillsCheckExamDutyProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int skillsCheckExamDutyProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSkillsCheckExamDutyProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SkillsCheckExamDutyProtocolId = skillsCheckExamDutyProtocolId,
            },
            ct);
}
