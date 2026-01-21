namespace SB.Api;

using MediatR;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IAbsencesByClassesReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class AbsencesByClassesReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IAbsencesByClassesReportsQueryRepository absencesByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await absencesByClassesReportsQueryRepository.GetAllAsync(
            schoolYear,
            instId,
            httpContextAccessor.GetSysUserId(),
            offset,
            limit,
            ct);

    [HttpPost]
    public async Task<ActionResult<int>> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateAbsencesByClassesReportCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        return await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
    }

    [HttpGet("{absencesByClassesReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByClassesReportId,
        [FromServices] IAbsencesByClassesReportsQueryRepository absencesByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await absencesByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, absencesByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await absencesByClassesReportsQueryRepository.GetAsync(schoolYear, instId, absencesByClassesReportId, ct);
    }

    [HttpGet("{absencesByClassesReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByClassesReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IAbsencesByClassesReportsQueryRepository absencesByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await absencesByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, absencesByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await absencesByClassesReportsQueryRepository.GetItemsAsync(schoolYear, instId, absencesByClassesReportId, offset, limit, ct);
    }

    [HttpDelete("{absencesByClassesReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByClassesReportId,
        [FromServices] IMediator mediator,
        [FromServices] IAbsencesByClassesReportsQueryRepository absencesByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await absencesByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, absencesByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveAbsencesByClassesReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AbsencesByClassesReportId = absencesByClassesReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{absencesByClassesReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByClassesReportId,
        [FromServices] IAbsencesByClassesReportsExcelExportService absencesByClassesReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await absencesByClassesReportsExcelExportService.ExportAsync(schoolYear, instId, absencesByClassesReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "отсъствия_по_класове.xlsx"
        };
    }
}
