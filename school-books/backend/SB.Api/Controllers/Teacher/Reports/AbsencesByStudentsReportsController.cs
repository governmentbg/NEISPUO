namespace SB.Api;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IAbsencesByStudentsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class AbsencesByStudentsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IAbsencesByStudentsReportsQueryRepository absencesByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await absencesByStudentsReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateAbsencesByStudentsReportCommand command,
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

    [HttpGet("{absencesByStudentsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByStudentsReportId,
        [FromServices] IAbsencesByStudentsReportsQueryRepository absencesByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await absencesByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, absencesByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await absencesByStudentsReportsQueryRepository.GetAsync(schoolYear, instId, absencesByStudentsReportId, ct);
    }

    [HttpGet("{absencesByStudentsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByStudentsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IAbsencesByStudentsReportsQueryRepository absencesByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await absencesByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, absencesByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await absencesByStudentsReportsQueryRepository.GetItemsAsync(schoolYear, instId, absencesByStudentsReportId, offset, limit, ct);
    }

    [HttpDelete("{absencesByStudentsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByStudentsReportId,
        [FromServices] IMediator mediator,
        [FromServices] IAbsencesByStudentsReportsQueryRepository absencesByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await absencesByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, absencesByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveAbsencesByStudentsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AbsencesByStudentsReportId = absencesByStudentsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{absencesByStudentsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int absencesByStudentsReportId,
        [FromServices] IAbsencesByStudentsReportsExcelExportService absencesByStudentsReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await absencesByStudentsReportsExcelExportService.ExportAsync(schoolYear, instId, absencesByStudentsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "отсъствия_по_ученици.xlsx"
        };
    }
}
