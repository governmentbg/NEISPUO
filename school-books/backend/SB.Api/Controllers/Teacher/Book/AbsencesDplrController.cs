namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IAbsencesDplrQueryRepository;

public class AbsencesDplrController : BookController
{
    [HttpGet]
    public async Task<ActionResult<GetAllDplrForClassBookVO[]>> GetAllForClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] AbsenceType type,
        [FromQuery] int? curriculumId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromServices] IAbsencesDplrQueryRepository absencesQueryRepository,
        CancellationToken ct)
        => await absencesQueryRepository.GetAllDplrForClassBookAsync(schoolYear, classBookId, type, curriculumId, fromDate, toDate, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllDplrForStudentAndTypeVO[]>> GetAllForStudentAndTypeAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int personId,
        [FromQuery] AbsenceType type,
        [FromQuery] int? curriculumId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromServices] IAbsencesDplrQueryRepository absencesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        int sysUserId = token.SelectedRole.SysUserId;
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var absences = await absencesQueryRepository.GetAllDplrForStudentAndTypeAsync(
            schoolYear,
            classBookId,
            personId,
            type,
            curriculumId,
            fromDate,
            toDate,
            ct);

        foreach (var absence in absences)
        {
            absence.HasUndoAccess =
                absence.CreatedBySysUserId == sysUserId;
            absence.HasRemoveAccess = hasClassBookAdminWriteAccess;
        }

        return absences;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllDplrForWeekVO[]>> GetAllForWeekAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int year,
        [FromQuery] int weekNumber,
        [FromQuery] AbsenceType type,
        [FromServices] IAbsencesDplrQueryRepository absencesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        int sysUserId = httpContextAccessor.GetSysUserId();

        var absences = await absencesQueryRepository.GetAllDplrForWeekAsync(schoolYear, classBookId, year, weekNumber, type, ct);

        foreach (var absence in absences)
        {
            absence.HasUndoAccess = absence.CreatedBySysUserId == sysUserId;
        }

        return absences;
    }
}
