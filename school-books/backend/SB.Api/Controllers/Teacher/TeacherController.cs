namespace SB.Api;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class TeacherController
{
    [HttpGet]
    public async Task<IClassBooksQueryRepository.GetAllVO[]> GetAllClassBooksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromServices] IClassBooksQueryRepository classBooksQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess = await httpContextAccessor.HasInstitutionAdminReadAccessAsync(instId, ct);
        int? selectedRolePersonId = httpContextAccessor.GetToken().SelectedRole.PersonId;

        return hasInstitutionAdminReadAccess
            ? await classBooksQueryRepository.GetAllAsync(schoolYear, instId, ct)
            : await classBooksQueryRepository.GetAllForTeacherAsync(
                schoolYear,
                instId,
                selectedRolePersonId ?? throw new Exception("The SelectedRole has no PersonId"),
                ct);
    }

    [HttpGet]
    public async Task<ActionResult<TableResultVO<IClassBooksAdminQueryRepository.GetAllVO>>> GetAllClassBooksAdminAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] ClassKind classKind,
        [FromQuery] string? bookName,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess = await httpContextAccessor.HasInstitutionAdminReadAccessAsync(instId, ct);
        int? selectedRolePersonId = httpContextAccessor.GetToken().SelectedRole.PersonId;

        return hasInstitutionAdminReadAccess
            ? await classBooksAdminQueryRepository.GetAllAsync(
                schoolYear,
                instId,
                classKind,
                bookName,
                offset,
                limit,
                ct)
            : await classBooksAdminQueryRepository.GetAllForTeacherAsync(
                schoolYear,
                instId,
                classKind,
                bookName,
                selectedRolePersonId ?? throw new Exception("The SelectedRole has no PersonId"),
                offset,
                limit,
                ct);
    }
}
