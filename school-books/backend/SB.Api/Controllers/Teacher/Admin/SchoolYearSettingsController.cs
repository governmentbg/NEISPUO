namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISchoolYearSettingsQueryRepository;

public class SchoolYearSettingsController : AdminController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository,
        CancellationToken ct)
        => await schoolYearSettingsQueryRepository.GetAllAsync(schoolYear, instId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateSchoolYearSettingsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]CreateSchoolYearSettingsCommand command,
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

    [HttpGet("{schoolYearSettingsId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int schoolYearSettingsId,
        [FromServices]ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository,
        CancellationToken ct)
        => await schoolYearSettingsQueryRepository.GetAsync(schoolYear, schoolYearSettingsId, ct);

    [HttpPost("{schoolYearSettingsId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int schoolYearSettingsId,
        [FromBody]UpdateSchoolYearSettingsCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SchoolYearSettingsId = schoolYearSettingsId,
            },
            ct);

    [HttpDelete("{schoolYearSettingsId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int schoolYearSettingsId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSchoolYearSettingsCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SchoolYearSettingsId = schoolYearSettingsId,
            },
            ct);
}
