namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IQualificationAcquisitionProtocolsQueryRepository;

public class QualificationAcquisitionProtocolsController : ProtocolsController
{
    [HttpGet("{qualificationAcquisitionProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromServices] IQualificationAcquisitionProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await еxamResultProtocolsQueryRepository.GetAsync(schoolYear, qualificationAcquisitionProtocolId, ct);

    [HttpGet("{qualificationAcquisitionProtocolId:int}")]
    public async Task<ActionResult<TableResultVO<GetStudentAllVO>>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IQualificationAcquisitionProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await еxamResultProtocolsQueryRepository.GetStudentAllAsync(schoolYear, qualificationAcquisitionProtocolId, offset, limit, ct);

    [HttpGet("{qualificationAcquisitionProtocolId:int}/{classId:int}/{personId:int}")]
    public async Task<ActionResult<GetStudentVO>> GetStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromRoute] int classId,
        [FromRoute] int personId,
        [FromServices] IQualificationAcquisitionProtocolsQueryRepository qualificationAcquisitionProtocolsQueryRepository,
        CancellationToken ct)
        => await qualificationAcquisitionProtocolsQueryRepository.GetStudentAsync(schoolYear, qualificationAcquisitionProtocolId, classId, personId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateQualificationAcquisitionProtocolCommand command,
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

    [HttpPost("{qualificationAcquisitionProtocolId:int}")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromBody] CreateQualificationAcquisitionProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                QualificationAcquisitionProtocolId = qualificationAcquisitionProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{qualificationAcquisitionProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromBody] UpdateQualificationAcquisitionProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationAcquisitionProtocolId = qualificationAcquisitionProtocolId,
            },
            ct);

    [HttpPost("{qualificationAcquisitionProtocolId:int}")]
    public async Task UpdateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromBody] UpdateQualificationAcquisitionProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationAcquisitionProtocolId = qualificationAcquisitionProtocolId,
            },
            ct);

    [HttpDelete("{qualificationAcquisitionProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveQualificationAcquisitionProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationAcquisitionProtocolId = qualificationAcquisitionProtocolId,
            },
            ct);

    [HttpDelete("{qualificationAcquisitionProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int qualificationAcquisitionProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveQualificationAcquisitionProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                QualificationAcquisitionProtocolId = qualificationAcquisitionProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
