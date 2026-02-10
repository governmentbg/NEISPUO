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
using static SB.Domain.IRegularGradePointAverageByClassesReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class RegularGradePointAverageByClassesReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IRegularGradePointAverageByClassesReportsQueryRepository regularGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await regularGradePointAverageByClassesReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateRegularGradePointAverageByClassesReportCommand command,
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

    [HttpGet("{regularGradePointAverageByClassesReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByClassesReportId,
        [FromServices] IRegularGradePointAverageByClassesReportsQueryRepository regularGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await regularGradePointAverageByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, regularGradePointAverageByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await regularGradePointAverageByClassesReportsQueryRepository.GetAsync(schoolYear, instId, regularGradePointAverageByClassesReportId, ct);
    }

    [HttpGet("{regularGradePointAverageByClassesReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByClassesReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IRegularGradePointAverageByClassesReportsQueryRepository regularGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await regularGradePointAverageByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, regularGradePointAverageByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await regularGradePointAverageByClassesReportsQueryRepository.GetItemsAsync(schoolYear, instId, regularGradePointAverageByClassesReportId, offset, limit, ct);
    }

    [HttpDelete("{regularGradePointAverageByClassesReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByClassesReportId,
        [FromServices] IMediator mediator,
        [FromServices] IRegularGradePointAverageByClassesReportsQueryRepository regularGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await regularGradePointAverageByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, regularGradePointAverageByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveRegularGradePointAverageByClassesReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                RegularGradePointAverageByClassesReportId = regularGradePointAverageByClassesReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{regularGradePointAverageByClassesReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByClassesReportId,
        [FromServices] IRegularGradePointAverageByClassesReportsExcelExportService regularGradePointAverageByClassesReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await regularGradePointAverageByClassesReportsExcelExportService.ExportAsync(schoolYear, instId, regularGradePointAverageByClassesReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "среден-успех-от-текущи-оценки-по-класове.xlsx"
        };
    }
}
