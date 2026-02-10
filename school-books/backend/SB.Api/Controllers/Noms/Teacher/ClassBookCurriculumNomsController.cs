namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

public class ClassBookCurriculumNomsController : BookNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromQuery][NotNull]int[] ids,
        [FromQuery]int? individualCurriculumPersonId,
        [FromQuery]int? gradeResultStudentPersonId,
        [FromServices]IClassBookCurriculumNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(
            schoolYear,
            instId,
            classBookId,
            individualCurriculumPersonId,
            gradeResultStudentPersonId,
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
        [FromQuery]int? individualCurriculumPersonId,
        [FromQuery]int? gradeResultStudentPersonId,
        [FromQuery]bool? excludeIndividualCurriculums,
        [FromServices]IClassBookCurriculumNomsRepository repository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        int? curriculumTeacherPersonId = null;

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

            curriculumTeacherPersonId =
                hasClassBookAdminWriteAccess
                    ? null
                    : token.SelectedRole.PersonId;
        }

        return await repository.GetNomsByTermAsync(
            schoolYear,
            instId,
            classBookId,
            curriculumTeacherPersonId,
            individualCurriculumPersonId,
            gradeResultStudentPersonId,
            excludeIndividualCurriculums,
            term,
            offset,
            limit,
            ct);
    }
}
