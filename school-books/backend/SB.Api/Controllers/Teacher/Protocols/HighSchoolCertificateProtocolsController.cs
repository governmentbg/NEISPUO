namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IHighSchoolCertificateProtocolQueryRepository;

public class HighSchoolCertificateProtocolsController : ProtocolsController
{
    [HttpGet("{highSchoolCertificateProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromServices] IHighSchoolCertificateProtocolQueryRepository highSchoolCertificateProtocolQueryRepository,
        CancellationToken ct)
        => await highSchoolCertificateProtocolQueryRepository.GetAsync(schoolYear, highSchoolCertificateProtocolId, ct);

    [HttpGet("{highSchoolCertificateProtocolId:int}")]
    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IHighSchoolCertificateProtocolQueryRepository highSchoolCertificateProtocolQueryRepository,
        CancellationToken ct)
        => await highSchoolCertificateProtocolQueryRepository.GetStudentAllAsync(schoolYear, highSchoolCertificateProtocolId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateHighSchoolCertificateProtocolCommand command,
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

    [HttpPost("{highSchoolCertificateProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromBody] UpdateHighSchoolCertificateProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                HighSchoolCertificateProtocolId = highSchoolCertificateProtocolId,
            },
            ct);

    [HttpPost("{highSchoolCertificateProtocolId:int}")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromBody] CreateHighSchoolCertificateProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                HighSchoolCertificateProtocolId = highSchoolCertificateProtocolId,
            },
            ct);

    [HttpPost("{highSchoolCertificateProtocolId:int}/students")]
    public async Task AddStudentsFromClassAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromBody] AddHighSchoolCertificateProtocolStudentsFromClassCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                HighSchoolCertificateProtocolId = highSchoolCertificateProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpDelete("{highSchoolCertificateProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveHighSchoolCertificateProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                HighSchoolCertificateProtocolId = highSchoolCertificateProtocolId,
            },
            ct);

    [HttpDelete("{highSchoolCertificateProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int highSchoolCertificateProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveHighSchoolCertificateProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                HighSchoolCertificateProtocolId = highSchoolCertificateProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
