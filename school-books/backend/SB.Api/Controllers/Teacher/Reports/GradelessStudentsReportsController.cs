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
using static SB.Domain.IGradelessStudentsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class GradelessStudentsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IGradelessStudentsReportsQueryRepository gradelessStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await gradelessStudentsReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateGradelessStudentsReportCommand command,
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

    [HttpGet("{gradelessStudentsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradelessStudentsReportId,
        [FromServices] IGradelessStudentsReportsQueryRepository gradelessStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await gradelessStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, gradelessStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await gradelessStudentsReportsQueryRepository.GetAsync(schoolYear, instId, gradelessStudentsReportId, ct);
    }

    [HttpGet("{gradelessStudentsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradelessStudentsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IGradelessStudentsReportsQueryRepository gradelessStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await gradelessStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, gradelessStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await gradelessStudentsReportsQueryRepository.GetItemsAsync(schoolYear, instId, gradelessStudentsReportId, offset, limit, ct);
    }

    [HttpDelete("{gradelessStudentsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradelessStudentsReportId,
        [FromServices] IMediator mediator,
        [FromServices] IGradelessStudentsReportsQueryRepository gradelessStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await gradelessStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, gradelessStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveGradelessStudentsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                GradelessStudentsReportId = gradelessStudentsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{gradelessStudentsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int gradelessStudentsReportId,
        [FromServices] IGradelessStudentsReportsExcelExportService gradelessStudentsReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await gradelessStudentsReportsExcelExportService.ExportAsync(schoolYear, instId, gradelessStudentsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "ученици_без_оценки.xlsx"
        };
    }
}
