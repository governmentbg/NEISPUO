namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using static SB.Data.IInstitutionStudentNomsRepository;

public class InstitutionStudentNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<InstitutionStudentNomVO[]> GetNomsByIdAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery][NotNull] InstitutionStudentNomVOStudent[] ids,
        [FromServices] IInstitutionStudentNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(
            schoolYear,
            instId,
            ids,
            ct);

    [HttpGet]
    public async Task<InstitutionStudentNomVO[]> GetNomsByTermAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromQuery] bool? showOnlyLastGrade,
        [FromServices] IInstitutionStudentNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(
            schoolYear,
            instId,
            term,
            offset,
            limit,
            showOnlyLastGrade,
            ct);
}
