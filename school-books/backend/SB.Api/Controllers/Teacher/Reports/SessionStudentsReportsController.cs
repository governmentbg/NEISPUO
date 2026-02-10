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
using static SB.Domain.ISessionStudentsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class SessionStudentsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ISessionStudentsReportsQueryRepository sessionStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await sessionStudentsReportsQueryRepository.GetAllAsync(
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
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
                new CreateSessionStudentsReportCommand
                {
                    SchoolYear = schoolYear,
                    InstId = instId,
                    SysUserId = httpContextAccessor.GetSysUserId(),
                },
                ct);

    [HttpGet("{sessionStudentsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int sessionStudentsReportId,
        [FromServices] ISessionStudentsReportsQueryRepository sessionStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await sessionStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, sessionStudentsReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await sessionStudentsReportsQueryRepository.GetAsync(schoolYear, instId, sessionStudentsReportId, ct);
    }

    [HttpGet("{sessionStudentsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int sessionStudentsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ISessionStudentsReportsQueryRepository sessionStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await sessionStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, sessionStudentsReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await sessionStudentsReportsQueryRepository.GetItemsAsync(schoolYear, instId, sessionStudentsReportId, offset, limit, ct);
    }

    [HttpDelete("{sessionStudentsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int sessionStudentsReportId,
        [FromServices] IMediator mediator,
        [FromServices] ISessionStudentsReportsQueryRepository sessionStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await sessionStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, sessionStudentsReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveSessionStudentsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SessionStudentsReportId = sessionStudentsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{sessionStudentsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int sessionStudentsReportId,
        [FromServices] ISessionStudentsReportsExcelExportService sessionStudentsReportExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await sessionStudentsReportExcelExportService.ExportAsync(schoolYear, instId, sessionStudentsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "ученици_за_поправителни_изпити.xlsx"
        };
    }
}
