namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using SB.Domain;

public class SupportActivityTypeNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery][NotNull] int[] ids,
        [FromServices] IScopedEntityNomsRepository<SupportActivityType> repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(schoolYear, instId, ids, ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IScopedEntityNomsRepository<SupportActivityType> repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(schoolYear, instId, term, offset, limit, ct);
}
