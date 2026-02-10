namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

public class ClassBookNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery][NotNull] int[] ids,
        [FromServices] IClassBookNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(schoolYear, instId, ids, ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] bool? showPG,
        [FromQuery] bool? showCdo,
        [FromQuery] bool? showDplr,
        [FromQuery] bool? showCsop,
        [FromQuery] bool? showInvalid,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IClassBookNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(schoolYear, instId, showPG, showCdo, showDplr, showCsop, showInvalid, term, offset, limit, ct);
}
