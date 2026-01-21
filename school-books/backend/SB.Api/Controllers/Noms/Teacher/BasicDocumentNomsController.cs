namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

public class BasicDocumentNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery][NotNull] int[] ids,
        [FromQuery] RegBookType type,
        [FromServices] IBasicDocumentNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(ids, type, ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromQuery] string? term,
        [FromQuery] RegBookType type,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IBasicDocumentNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(term, type, offset, limit, ct);
}
