namespace SB.Api;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class ScheduleLessonNomsController : BookNomsController
{
    [HttpGet]
    public async Task<ScheduleLessonNomVO[]> GetNomsAsync(
        [FromRoute] int instId,
        [FromRoute] int schoolYear,
        [FromRoute] int classBookId,
        [FromQuery] int curriculumId,
        [FromQuery] DateTime date,
        [FromServices] IScheduleLessonNomsRepository repository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        bool hasClassBookAdminWriteAccess = await authService.HasClassBookAdminAccessAsync(
            token,
            AccessType.Write,
            schoolYear,
            instId,
            classBookId,
            ct);

        int? curriculumTeacherPersonId = hasClassBookAdminWriteAccess ? null : token.SelectedRole.PersonId;
        return await repository.GetNomsAsync(schoolYear, instId, classBookId, curriculumId, date.Date, curriculumTeacherPersonId, ct);
    }
}
