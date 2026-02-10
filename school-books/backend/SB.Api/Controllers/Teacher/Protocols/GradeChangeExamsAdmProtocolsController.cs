namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IGradeChangeExamsAdmProtocolQueryRepository;

public class GradeChangeExamsAdmProtocolsController : ProtocolsController
{
    [HttpGet("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromServices] IGradeChangeExamsAdmProtocolQueryRepository gradeChangeExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await gradeChangeExamsAdmProtocolQueryRepository.GetAsync(schoolYear, gradeChangeExamsAdmProtocolId, ct);

    [HttpGet("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices]IGradeChangeExamsAdmProtocolQueryRepository gradeChangeExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await gradeChangeExamsAdmProtocolQueryRepository.GetStudentAllAsync(schoolYear, gradeChangeExamsAdmProtocolId, offset, limit, ct);

    [HttpGet("{gradeChangeExamsAdmProtocolId:int}/{classId:int}/{personId:int}")]
    public async Task<ActionResult<GetStudentVO>> GetStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromRoute] int classId,
        [FromRoute] int personId,
        [FromServices]IGradeChangeExamsAdmProtocolQueryRepository gradeChangeExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await gradeChangeExamsAdmProtocolQueryRepository.GetStudentAsync(schoolYear, gradeChangeExamsAdmProtocolId, classId, personId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody]CreateGradeChangeExamsAdmProtocolCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromBody]UpdateGradeChangeExamsAdmProtocolCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeChangeExamsAdmProtocolId = gradeChangeExamsAdmProtocolId,
            },
            ct);

    [HttpPost("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromBody]CreateGradeChangeExamsAdmProtocolStudentCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeChangeExamsAdmProtocolId = gradeChangeExamsAdmProtocolId,
            },
            ct);

    [HttpPost("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task UpdateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromBody]UpdateGradeChangeExamsAdmProtocolStudentCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeChangeExamsAdmProtocolId = gradeChangeExamsAdmProtocolId,
            },
            ct);

    [HttpDelete("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradeChangeExamsAdmProtocolId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveGradeChangeExamsAdmProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeChangeExamsAdmProtocolId = gradeChangeExamsAdmProtocolId,
            },
            ct);

    [HttpDelete("{gradeChangeExamsAdmProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int gradeChangeExamsAdmProtocolId,
        [FromQuery]int classId,
        [FromQuery]int personId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveGradeChangeExamsAdmProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradeChangeExamsAdmProtocolId = gradeChangeExamsAdmProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
