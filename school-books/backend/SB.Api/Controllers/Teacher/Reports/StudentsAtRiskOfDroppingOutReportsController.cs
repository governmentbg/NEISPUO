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
using static SB.Domain.IStudentsAtRiskOfDroppingOutReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class StudentsAtRiskOfDroppingOutReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentsAtRiskOfDroppingOutReportsQueryRepository studentsAtRiskOfDroppingOutReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await studentsAtRiskOfDroppingOutReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateStudentsAtRiskOfDroppingOutReportCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
                command with
                {
                    SchoolYear = schoolYear,
                    InstId = instId,
                    SysUserId = httpContextAccessor.GetSysUserId()
                },
                ct);


    [HttpGet("{studentsAtRiskOfDroppingOutReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int studentsAtRiskOfDroppingOutReportId,
        [FromServices] IStudentsAtRiskOfDroppingOutReportsQueryRepository studentsAtRiskOfDroppingOutReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await studentsAtRiskOfDroppingOutReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, studentsAtRiskOfDroppingOutReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await studentsAtRiskOfDroppingOutReportsQueryRepository.GetAsync(schoolYear, instId, studentsAtRiskOfDroppingOutReportId, ct);
    }

    [HttpGet("{studentsAtRiskOfDroppingOutReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int studentsAtRiskOfDroppingOutReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentsAtRiskOfDroppingOutReportsQueryRepository studentsAtRiskOfDroppingOutReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await studentsAtRiskOfDroppingOutReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, studentsAtRiskOfDroppingOutReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await studentsAtRiskOfDroppingOutReportsQueryRepository.GetItemsAsync(schoolYear, instId, studentsAtRiskOfDroppingOutReportId, offset, limit, ct);
    }

    [HttpDelete("{studentsAtRiskOfDroppingOutReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int studentsAtRiskOfDroppingOutReportId,
        [FromServices] IMediator mediator,
        [FromServices] IStudentsAtRiskOfDroppingOutReportsQueryRepository studentsAtRiskOfDroppingOutReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await studentsAtRiskOfDroppingOutReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, studentsAtRiskOfDroppingOutReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveStudentsAtRiskOfDroppingOutReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StudentsAtRiskOfDroppingOutReportId = studentsAtRiskOfDroppingOutReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{studentsAtRiskOfDroppingOutReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int studentsAtRiskOfDroppingOutReportId,
        [FromServices] IStudentsAtRiskOfDroppingOutReportsExcelExportService studentsAtRiskOfDroppingOutReportExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await studentsAtRiskOfDroppingOutReportExcelExportService.ExportAsync(schoolYear, instId, studentsAtRiskOfDroppingOutReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "ученици_с_риск_от_отпадане.xlsx"
        };
    }
}
