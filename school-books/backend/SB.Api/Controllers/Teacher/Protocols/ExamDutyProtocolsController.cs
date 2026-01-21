namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IExamDutyProtocolsQueryRepository;

public class ExamDutyProtocolsController : ProtocolsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] ExamDutyProtocolType? protocolType,
        [FromQuery] string? orderNumber,
        [FromQuery] DateTime? orderDate,
        [FromQuery] DateTime? protocolDate,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IExamDutyProtocolsQueryRepository examDutyProtocolsQueryRepository,
        CancellationToken ct)
    {
        return await examDutyProtocolsQueryRepository.GetAllAsync(
            instId,
            schoolYear,
            protocolType,
            orderNumber,
            orderDate,
            protocolDate,
            offset,
            limit,
            ct);
    }

    [HttpGet("{examDutyProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromServices] IExamDutyProtocolsQueryRepository examDutyProtocolsQueryRepository,
        CancellationToken ct)
        => await examDutyProtocolsQueryRepository.GetAsync(schoolYear, examDutyProtocolId, ct);

    [HttpGet("{examDutyProtocolId:int}/students")]
    public async Task<ActionResult<TableResultVO<GetStudentAllVO>>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IExamDutyProtocolsQueryRepository examDutyProtocolsQueryRepository,
        CancellationToken ct)
        => await examDutyProtocolsQueryRepository.GetStudentAllAsync(schoolYear, examDutyProtocolId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateExamDutyProtocolCommand command,
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

    [HttpPost("{examDutyProtocolId:int}/students")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromBody] CreateExamDutyProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ExamDutyProtocolId = examDutyProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{examDutyProtocolId:int}/students")]
    public async Task AddStudentsFromClassAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromBody] AddExamDutyProtocolStudentsFromClassCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ExamDutyProtocolId = examDutyProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{examDutyProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromBody] UpdateExamDutyProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamDutyProtocolId = examDutyProtocolId,
            },
            ct);

    [HttpDelete("{examDutyProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveExamDutyProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamDutyProtocolId = examDutyProtocolId,
            },
            ct);

    [HttpDelete("{examDutyProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examDutyProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveExamDutyProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamDutyProtocolId = examDutyProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
