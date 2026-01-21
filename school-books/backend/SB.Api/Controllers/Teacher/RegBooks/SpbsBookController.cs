namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISpbsBookRecordsQueryRepository;

public class SpbsBookController : RegBooksController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int? recordSchoolYear,
        [FromQuery]int? recordNumber,
        [FromQuery]string? studentName,
        [FromQuery]string? personalId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository,
        CancellationToken ct)
        => await spbsBookRecordsQueryRepository.GetAllAsync(
            instId,
            recordSchoolYear,
            recordNumber,
            studentName,
            personalId,
            offset,
            limit,
            ct);

    [HttpGet("{spbsBookRecordId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromServices]ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository,
        CancellationToken ct)
        => await spbsBookRecordsQueryRepository.GetAsync(schoolYear, spbsBookRecordId, ct);

    [HttpGet("{spbsBookRecordId:int}/escapes")]
    public async Task<ActionResult<TableResultVO<GetEscapeVO>>> GetEscapeAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository,
        CancellationToken ct)
        => await spbsBookRecordsQueryRepository.GetEscapeAllAsync(schoolYear, spbsBookRecordId, offset, limit, ct);

    [HttpGet("{spbsBookRecordId:int}/escapes/{orderNum:int}")]
    public async Task<ActionResult<GetEscapeVO>> GetEscapeAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromRoute]int orderNum,
        [FromServices]ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository,
        CancellationToken ct)
        => await spbsBookRecordsQueryRepository.GetEscapeAsync(schoolYear, spbsBookRecordId, orderNum, ct);

    [HttpGet("{spbsBookRecordId:int}/absences")]
    public async Task<ActionResult<TableResultVO<GetAbsenceVO>>> GetAbsenceAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository,
        CancellationToken ct)
        => await spbsBookRecordsQueryRepository.GetAbsenceAllAsync(schoolYear, spbsBookRecordId, offset, limit, ct);

    [HttpGet("{spbsBookRecordId:int}/absences/{orderNum:int}")]
    public async Task<ActionResult<GetAbsenceVO>> GetAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromRoute]int orderNum,
        [FromServices]ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository,
        CancellationToken ct)
        => await spbsBookRecordsQueryRepository.GetAbsenceAsync(schoolYear, spbsBookRecordId, orderNum, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]CreateSpbsBookRecordCommand command,
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

    [HttpPost("{spbsBookRecordId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromBody]UpdateSpbsBookRecordCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
            },
            ct);

    [HttpDelete("{spbsBookRecordId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int spbsBookRecordId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSpbsBookRecordCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
            },
            ct);

    [HttpPost("{spbsBookRecordId:int}/escapes")]
    public async Task CreateEscapeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromBody]CreateSpbsBookRecordEscapeCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
            },
            ct);

    [HttpPost("{spbsBookRecordId:int}/escapes/{orderNum:int}")]
    public async Task UpdateEscapeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromRoute]int orderNum,
        [FromBody]UpdateSpbsBookRecordEscapeCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
                OrderNum = orderNum
            },
            ct);

    [HttpDelete("{spbsBookRecordId:int}/escapes/{orderNum:int}")]
    public async Task RemoveEscapeAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int spbsBookRecordId,
        [FromRoute] int orderNum,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSpbsBookRecordEscapeCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
                OrderNum = orderNum
            },
            ct);

    [HttpPost("{spbsBookRecordId:int}/absences")]
    public async Task CreateAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromBody]CreateSpbsBookRecordAbsenceCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
            },
            ct);

    [HttpPost("{spbsBookRecordId:int}/absences/{orderNum:int}")]
    public async Task UpdateAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int spbsBookRecordId,
        [FromRoute]int orderNum,
        [FromBody]UpdateSpbsBookRecordAbsenceCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
                OrderNum = orderNum
            },
            ct);

    [HttpDelete("{spbsBookRecordId:int}/absences/{orderNum:int}")]
    public async Task RemoveAbsenceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int spbsBookRecordId,
        [FromRoute] int orderNum,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSpbsBookRecordAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SpbsBookRecordId = spbsBookRecordId,
                OrderNum = orderNum
            },
            ct);
}
