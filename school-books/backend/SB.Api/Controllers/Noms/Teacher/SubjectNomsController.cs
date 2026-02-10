namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

public class SubjectNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery][NotNull] int[] ids,
        [FromServices] ISubjectNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(ids, ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ISubjectNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(schoolYear, instId, term, offset, limit, ct);
}
