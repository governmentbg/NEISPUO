namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using SB.Domain;

public class SubjectTypeNomsController : CommonNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromQuery][NotNull] int[] ids,
        [FromServices] IEntityNomsRepository<SubjectType> repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(ids, ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IEntityNomsRepository<SubjectType> repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(term, offset, limit, ct);
}
