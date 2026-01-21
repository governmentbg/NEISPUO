namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IQualificationExamResultProtocolsQueryRepository;

public class QualificationExamResultProtocolsController : ProtocolsController
{
    [HttpGet("{qualificationExamResultProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromServices] IQualificationExamResultProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await еxamResultProtocolsQueryRepository.GetAsync(schoolYear, qualificationExamResultProtocolId, ct);

    [HttpGet("{qualificationExamResultProtocolId:int}")]
    public async Task<ActionResult<TableResultVO<GetStudentAllVO>>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IQualificationExamResultProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await еxamResultProtocolsQueryRepository.GetStudentAllAsync(schoolYear, qualificationExamResultProtocolId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateQualificationExamResultProtocolCommand command,
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

    [HttpPost("{qualificationExamResultProtocolId:int}")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromBody] CreateQualificationExamResultProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                QualificationExamResultProtocolId = qualificationExamResultProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{qualificationExamResultProtocolId:int}/students")]
    public async Task AddStudentsFromClassAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromBody] AddQualificationExamResultProtocolStudentsFromClassCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                QualificationExamResultProtocolId = qualificationExamResultProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{qualificationExamResultProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromBody] UpdateQualificationExamResultProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationExamResultProtocolId = qualificationExamResultProtocolId,
            },
            ct);

    [HttpDelete("{qualificationExamResultProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveQualificationExamResultProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationExamResultProtocolId = qualificationExamResultProtocolId,
            },
            ct);

    [HttpDelete("{qualificationExamResultProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationExamResultProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveQualificationExamResultProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationExamResultProtocolId = qualificationExamResultProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
