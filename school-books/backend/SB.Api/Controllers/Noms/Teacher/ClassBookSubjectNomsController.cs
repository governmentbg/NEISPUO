namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

public class ClassBookSubjectNomsController : BookNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromQuery][NotNull]int[] ids,
        [FromServices]IClassBookSubjectNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(
            schoolYear,
            instId,
            classBookId,
            ids,
            ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromQuery]string? term,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromQuery]bool? withWriteAccess,
        [FromServices]IClassBookSubjectNomsRepository repository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        int? writeAccessCurriculumTeacherPersonId = null;

        if (withWriteAccess == true)
        {
            OidcToken token = httpContextAccessor.GetToken();
            bool hasClassBookAdminWriteAccess = await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

            writeAccessCurriculumTeacherPersonId =
                hasClassBookAdminWriteAccess
                    ? null
                    : token.SelectedRole.PersonId;
        }

        return await repository.GetNomsByTermAsync(
            schoolYear,
            instId,
            classBookId,
            writeAccessCurriculumTeacherPersonId,
            term,
            offset,
            limit,
            ct);
    }
}
