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
using static SB.Domain.IRegularGradePointAverageByStudentsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class RegularGradePointAverageByStudentsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IRegularGradePointAverageByStudentsReportsQueryRepository regularGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await regularGradePointAverageByStudentsReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateRegularGradePointAverageByStudentsReportCommand command,
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

    [HttpGet("{regularGradePointAverageByStudentsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByStudentsReportId,
        [FromServices] IRegularGradePointAverageByStudentsReportsQueryRepository regularGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await regularGradePointAverageByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, regularGradePointAverageByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await regularGradePointAverageByStudentsReportsQueryRepository.GetAsync(schoolYear, instId, regularGradePointAverageByStudentsReportId, ct);
    }

    [HttpGet("{regularGradePointAverageByStudentsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByStudentsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IRegularGradePointAverageByStudentsReportsQueryRepository regularGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await regularGradePointAverageByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, regularGradePointAverageByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await regularGradePointAverageByStudentsReportsQueryRepository.GetItemsAsync(schoolYear, instId, regularGradePointAverageByStudentsReportId, offset, limit, ct);
    }

    [HttpDelete("{regularGradePointAverageByStudentsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByStudentsReportId,
        [FromServices] IMediator mediator,
        [FromServices] IRegularGradePointAverageByStudentsReportsQueryRepository regularGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await regularGradePointAverageByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, regularGradePointAverageByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveRegularGradePointAverageByStudentsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                RegularGradePointAverageByStudentsReportId = regularGradePointAverageByStudentsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{regularGradePointAverageByStudentsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int regularGradePointAverageByStudentsReportId,
        [FromServices] IRegularGradePointAverageByStudentsReportsExcelExportService regularGradePointAverageByStudentsReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await regularGradePointAverageByStudentsReportsExcelExportService.ExportAsync(schoolYear, instId, regularGradePointAverageByStudentsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "среден-успех-от-текущи-оценки-по-ученици.xlsx"
        };
    }
}
