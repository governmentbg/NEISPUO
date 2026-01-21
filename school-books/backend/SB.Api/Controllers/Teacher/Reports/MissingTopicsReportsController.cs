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
using static SB.Domain.IMissingTopicsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class MissingTopicsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IMissingTopicsReportsQueryRepository missingTopicsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await missingTopicsReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateMissingTopicsReportCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        bool hasInstitutionAdminWriteAccess = await httpContextAccessor.HasInstitutionAdminWriteAccessAsync(instId, ct);
        int? selectedRolePersonId = httpContextAccessor.GetToken().SelectedRole.PersonId;

        if (!hasInstitutionAdminWriteAccess &&
            (command.TeacherPersonId == null
            || command.TeacherPersonId != selectedRolePersonId))
        {
            return new ForbidResult();
        }

        return await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
    }

    [HttpGet("{missingTopicsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int missingTopicsReportId,
        [FromServices] IMissingTopicsReportsQueryRepository missingTopicsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await missingTopicsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, missingTopicsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await missingTopicsReportsQueryRepository.GetAsync(schoolYear, instId, missingTopicsReportId, ct);
    }

    [HttpGet("{missingTopicsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int missingTopicsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IMissingTopicsReportsQueryRepository missingTopicsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await missingTopicsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, missingTopicsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await missingTopicsReportsQueryRepository.GetItemsAsync(schoolYear, instId, missingTopicsReportId, offset, limit, ct);
    }

    [HttpDelete("{missingTopicsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int missingTopicsReportId,
        [FromServices] IMediator mediator,
        [FromServices] IMissingTopicsReportsQueryRepository missingTopicsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await missingTopicsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, missingTopicsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveMissingTopicsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                MissingTopicsReportId = missingTopicsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{missingTopicsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int missingTopicsReportId,
        [FromServices] IMissingTopicsReportsExcelExportService missingTopicsReportsQueryRepository,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await missingTopicsReportsQueryRepository.ExportAsync(schoolYear, instId, missingTopicsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "невписани_теми.xlsx"
        };
    }
}
