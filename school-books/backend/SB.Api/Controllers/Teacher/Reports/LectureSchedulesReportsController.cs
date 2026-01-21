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
using static SB.Domain.ILectureSchedulesReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class LectureSchedulesReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ILectureSchedulesReportsQueryRepository lectureSchedulesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await lectureSchedulesReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateLectureSchedulesReportCommand command,
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

    [HttpGet("{lectureSchedulesReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureSchedulesReportId,
        [FromServices] ILectureSchedulesReportsQueryRepository lectureSchedulesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await lectureSchedulesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, lectureSchedulesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await lectureSchedulesReportsQueryRepository.GetAsync(schoolYear, instId, lectureSchedulesReportId, ct);
    }

    [HttpGet("{lectureSchedulesReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureSchedulesReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ILectureSchedulesReportsQueryRepository lectureSchedulesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await lectureSchedulesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, lectureSchedulesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await lectureSchedulesReportsQueryRepository.GetItemsAsync(schoolYear, instId, lectureSchedulesReportId, offset, limit, ct);
    }

    [HttpDelete("{lectureSchedulesReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureSchedulesReportId,
        [FromServices] IMediator mediator,
        [FromServices] ILectureSchedulesReportsQueryRepository lectureSchedulesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await lectureSchedulesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, lectureSchedulesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveLectureSchedulesReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                LectureSchedulesReportId = lectureSchedulesReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{lectureSchedulesReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureSchedulesReportId,
        [FromServices] ILectureSchedulesReportsExcelExportService lectureSchedulesReportExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await lectureSchedulesReportExcelExportService.ExportAsync(schoolYear, instId, lectureSchedulesReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "лекторски_часове.xlsx"
        };
    }
}
