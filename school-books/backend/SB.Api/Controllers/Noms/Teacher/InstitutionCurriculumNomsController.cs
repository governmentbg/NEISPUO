namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using static SB.Data.IInstitutionCurriculumNomsRepository;

public class InstitutionCurriculumNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<InstitutionCurriculumNomVO[]> GetNomsByIdAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery][NotNull] InstitutionCurriculumNomVOCurriculum[] ids,
        [FromServices] IInstitutionCurriculumNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(
            schoolYear,
            instId,
            ids,
            ct);

    [HttpGet]
    public async Task<InstitutionCurriculumNomVO[]> GetNomsByTermAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IInstitutionCurriculumNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(
            schoolYear,
            instId,
            term,
            offset,
            limit,
            ct);
}
