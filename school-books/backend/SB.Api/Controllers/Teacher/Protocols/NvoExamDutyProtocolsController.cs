namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.INvoExamDutyProtocolsQueryRepository;

public class NvoExamDutyProtocolsController : ProtocolsController
{
    [HttpGet("{nvoExamDutyProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromServices] INvoExamDutyProtocolsQueryRepository nvoExamDutyProtocolsQueryRepository,
        CancellationToken ct)
        => await nvoExamDutyProtocolsQueryRepository.GetAsync(schoolYear, nvoExamDutyProtocolId, ct);

    [HttpGet("{nvoExamDutyProtocolId:int}")]
    public async Task<ActionResult<TableResultVO<GetStudentAllVO>>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] INvoExamDutyProtocolsQueryRepository nvoExamDutyProtocolsQueryRepository,
        CancellationToken ct)
        => await nvoExamDutyProtocolsQueryRepository.GetStudentAllAsync(schoolYear, nvoExamDutyProtocolId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateNvoExamDutyProtocolCommand command,
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

    [HttpPost("{nvoExamDutyProtocolId:int}/students")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromBody] CreateNvoExamDutyProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                NvoExamDutyProtocolId = nvoExamDutyProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{nvoExamDutyProtocolId:int}/students")]
    public async Task AddStudentsFromClassAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromBody] AddNvoExamDutyProtocolStudentsFromClassCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                NvoExamDutyProtocolId = nvoExamDutyProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{nvoExamDutyProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromBody] UpdateNvoExamDutyProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                NvoExamDutyProtocolId = nvoExamDutyProtocolId,
            },
            ct);

    [HttpDelete("{nvoExamDutyProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveNvoExamDutyProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                NvoExamDutyProtocolId = nvoExamDutyProtocolId,
            },
            ct);

    [HttpDelete("{nvoExamDutyProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int nvoExamDutyProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveNvoExamDutyProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                NvoExamDutyProtocolId = nvoExamDutyProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
