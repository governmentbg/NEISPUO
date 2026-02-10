namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IGraduationThesisDefenseProtocolQueryRepository;

public class GraduationThesisDefenseProtocolsController : ProtocolsController
{
    [HttpGet("{graduationThesisDefenseProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int graduationThesisDefenseProtocolId,
        [FromServices] IGraduationThesisDefenseProtocolQueryRepository graduationThesisDefenseProtocolQueryRepository,
        CancellationToken ct)
        => await graduationThesisDefenseProtocolQueryRepository.GetAsync(schoolYear, graduationThesisDefenseProtocolId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateGraduationThesisDefenseProtocolCommand command,
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

    [HttpPost("{graduationThesisDefenseProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int graduationThesisDefenseProtocolId,
        [FromBody] UpdateGraduationThesisDefenseProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GraduationThesisDefenseProtocolId = graduationThesisDefenseProtocolId,
            },
            ct);

    [HttpDelete("{graduationThesisDefenseProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int graduationThesisDefenseProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveGraduationThesisDefenseProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GraduationThesisDefenseProtocolId = graduationThesisDefenseProtocolId,
            },
            ct);
}
