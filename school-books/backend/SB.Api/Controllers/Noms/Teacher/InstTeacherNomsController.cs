namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using static SB.Data.IInstTeacherNomsRepository;

public class InstTeacherNomsController : InstitutionNomsController
{
    [HttpGet]
    public async Task<InstTeacherNomVO[]> GetNomsByIdAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute]int instId,
        [FromQuery][NotNull]int[] ids,
        [FromServices]IInstTeacherNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(instId, ids, ct);

    [HttpGet]
    public async Task<InstTeacherNomVO[]> GetNomsByTermAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]string? term,
        [FromQuery]bool? includeNonPedagogical,
        [FromQuery]bool? includeNotActiveTeachers,
        [FromQuery]bool? includeNoReplacementTeachers,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IInstTeacherNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(
            instId,
            schoolYear,
            term,
            includeNonPedagogical,
            includeNotActiveTeachers,
            includeNoReplacementTeachers,
            offset,
            limit,
            ct);
}
