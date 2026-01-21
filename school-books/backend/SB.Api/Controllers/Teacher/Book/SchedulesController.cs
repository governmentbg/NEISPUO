namespace SB.Api;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISchedulesQueryRepository;

public class SchedulesController : BookAdminController
{
    [HttpGet("{year:int}/{weekNumber:int}")]
    public async Task<ActionResult<GetClassBookScheduleForWeekVO>> GetClassBookScheduleForWeekAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int year,
        [FromRoute] int weekNumber,
        [FromQuery] int? personId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        int? selectedRolePersonId = token.SelectedRole.PersonId;
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var schedule =
            await schedulesQueryRepository.GetClassBookScheduleForWeekAsync(
                schoolYear,
                classBookId,
                year,
                weekNumber,
                personId,
                ct);

        foreach (var scheduleHour in schedule.Hours)
        {
            scheduleHour.HasAbsenceCreateAccess =
            scheduleHour.HasTopicCreateAccess =
                hasClassBookAdminWriteAccess
                || scheduleHour.CurriculumTeachers.Any(ctpi => ctpi.TeacherPersonId == selectedRolePersonId)
                || scheduleHour.ReplTeacher?.TeacherPersonId == selectedRolePersonId;
            scheduleHour.HasAbsenceExcuseAccess =
            scheduleHour.HasAbsenceRemoveAccess =
                hasClassBookAdminWriteAccess;
        }

        return schedule;
    }

    [HttpGet("{year:int}/{weekNumber:int}")]
    public async Task<ActionResult<GetClassBookScheduleTableForWeekVO>> GetClassBookScheduleTableForWeekAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int year,
        [FromRoute] int weekNumber,
        [FromQuery] int? personId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetClassBookScheduleTableForWeekAsync(
            schoolYear,
            classBookId,
            year,
            weekNumber,
            showIndividualCurriculum: personId.HasValue,
            personId,
            ct: ct);

    [HttpGet]
    public async Task<ActionResult<GetSchoolYearSettings>> GetSchoolYearSettingsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetSchoolYearSettingsAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllByClassBookVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] bool isIndividualSchedule,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetAllByClassBookAsync(schoolYear, classBookId, isIndividualSchedule, offset, limit, ct);

    [HttpGet("{scheduleId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int scheduleId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetAsync(schoolYear, instId, classBookId, scheduleId, ct);

    [HttpGet]
    public async Task<ActionResult<GetUsedDatesWeeksVO>> GetUsedDatesWeeksAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] bool isIndividualSchedule,
        [FromQuery] int? personId,
        [FromQuery] int? exceptScheduleId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetUsedDatesWeeksAsync(schoolYear, classBookId, isIndividualSchedule, personId, exceptScheduleId, ct);

    [HttpGet]
    public async Task<ActionResult<DateTime[]>> GetOffDatesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetOffDatesAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetOffDatesPgVO[]>> GetOffDatesPgAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetOffDatesPgAsync(schoolYear, classBookId, ct);

    [HttpPost]
    public async Task CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] CreateScheduleCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{scheduleId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int scheduleId,
        [FromBody] UpdateScheduleCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleId = scheduleId,
            },
            ct);

    [HttpDelete("{scheduleId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int scheduleId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveScheduleCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleId = scheduleId,
            },
            ct);

    [HttpPost("{scheduleId:int}")]
    public async Task SplitAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int scheduleId,
        [FromBody] SplitScheduleCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleId = scheduleId,
            },
            ct);

    [HttpPost("{scheduleId:int}/{shiftId:int}")]
    public async Task<string?> CanChangeShiftWithAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute] int scheduleId,
        [FromRoute] int shiftId,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        [FromServices] IShiftsQueryRepository shiftsQueryRepository,
        CancellationToken ct)
    {
        var newShiftHours =
            (await schedulesQueryRepository.GetShiftHoursForValidationAsync(
                schoolYear,
                instId,
                shiftId,
                ct))
            .Select(sh => (sh.Day, sh.HourNumber))
            .ToArray();
        var usedHourNumbers =
            (await shiftsQueryRepository.GetHoursUsedInScheduleAsync(
                schoolYear,
                scheduleId,
                ct))
            .Select(h => (h.Day, h.HourNumber));

        HashSet<(int, int)> missing = new(usedHourNumbers);
        missing.ExceptWith(newShiftHours);
        if (missing.Count > 0)
        {
            var shiftInfo = await shiftsQueryRepository.GetShiftInfoAsync(schoolYear, instId, shiftId, ct);
            (int missingDay, int missingHourNumber) = missing.First();
            if (shiftInfo.IsMultiday)
            {
                return $"Смяната \"{shiftInfo.Name}\" не съдържа {GetOrdinal(missingHourNumber)} час в {DateExtensions.GetLocalizedDayName(missingDay)}, който се използва в разписанието.";
            }
            else
            {
                return $"Смяната \"{shiftInfo.Name}\" не съдържа {GetOrdinal(missingHourNumber)} час, който се използва в разписанието.";
            }
        }

        return null;
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{scheduleId:int}/{day:int}")]
    public async Task<FileStreamResult> DownloadScheduleUsedHoursTableExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int scheduleId,
        [FromRoute] int day,
        [FromServices] ISchedulesExcelService schedulesExcelService,
        CancellationToken ct)
    {
        MemoryStream excelStream = new(); // this stream is closed from asp.net

        await schedulesExcelService.GetScheduleUsedHoursTableAsync(
            schoolYear,
            instId,
            classBookId,
            scheduleId,
            day,
            excelStream,
            ct);

        excelStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(excelStream, OpenXmlExtensions.ExcelMimeType) { FileDownloadName = "заключени_часове.xlsx" };
    }

    [HttpGet]
    public async Task<ActionResult<GetScheduleCurriculumInfoVO[]>> GetScheduleCurriculumsInfoAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromQuery] int[] curriculumIds,
        [FromServices] ISchedulesQueryRepository schedulesQueryRepository,
        CancellationToken ct)
        => await schedulesQueryRepository.GetScheduleCurriculumsInfoAsync(schoolYear, instId, curriculumIds, ct);

    private static string GetOrdinal(int number)
        => $"{number}{GetOrdinalSuffix(number)}";

    private static string GetOrdinalSuffix(int number)
        => number < 0
            ? ""
            : (number % 100) switch
            {
                0 => "",
                11 or 12 or 17 or 18 => "ти",
                int n =>
                    (n % 10) switch
                    {
                        1 => "ви",
                        2 => "ри",
                        7 or 8 => "ми",
                        _ => "ти",
                    }
            };
}
