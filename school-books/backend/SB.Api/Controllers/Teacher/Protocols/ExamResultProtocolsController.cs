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
using static SB.Domain.IExamResultProtocolsQueryRepository;

public class ExamResultProtocolsController : ProtocolsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] ExamResultProtocolType? protocolType,
        [FromQuery] string? orderNumber,
        [FromQuery] string? protocolNumber,
        [FromQuery] DateTime? protocolDate,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IExamResultProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
    {
        return await еxamResultProtocolsQueryRepository.GetAllAsync(
            instId,
            schoolYear,
            orderNumber,
            protocolType,
            protocolNumber,
            protocolDate,
            offset,
            limit,
            ct);
    }

    [HttpGet("{examResultProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int examResultProtocolId,
        [FromServices] IExamResultProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await еxamResultProtocolsQueryRepository.GetAsync(schoolYear, examResultProtocolId, ct);

    [HttpGet("{examResultProtocolId:int}")]
    public async Task<ActionResult<TableResultVO<GetStudentAllVO>>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int examResultProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IExamResultProtocolsQueryRepository еxamResultProtocolsQueryRepository,
        CancellationToken ct)
        => await еxamResultProtocolsQueryRepository.GetStudentAllAsync(schoolYear, examResultProtocolId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateExamResultProtocolCommand command,
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

    [HttpPost("{examResultProtocolId:int}")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examResultProtocolId,
        [FromBody] CreateExamResultProtocolStudentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ExamResultProtocolId = examResultProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{examResultProtocolId:int}/students")]
    public async Task AddStudentsFromClassAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examResultProtocolId,
        [FromBody] AddExamResultProtocolStudentsFromClassCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ExamResultProtocolId = examResultProtocolId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{examResultProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examResultProtocolId,
        [FromBody] UpdateExamResultProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamResultProtocolId = examResultProtocolId,
            },
            ct);

    [HttpDelete("{examResultProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examResultProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveExamResultProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamResultProtocolId = examResultProtocolId,
            },
            ct);

    [HttpDelete("{examResultProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examResultProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveExamResultProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamResultProtocolId = examResultProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
