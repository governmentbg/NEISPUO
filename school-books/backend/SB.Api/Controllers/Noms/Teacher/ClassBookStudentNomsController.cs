namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using static SB.Data.IClassBookStudentNomsRepository;

public class ClassBookStudentNomsController : BookNomsController
{
    [HttpGet]
    public async Task<ClassBookStudentNomVO[]> GetNomsByIdAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromQuery][NotNull] int[] ids,
        [FromQuery] bool? showOnlyWithIndividualCurriculum,
        [FromQuery] bool? showOnlyWithIndividualCurriculumSchedule,
        [FromServices] IClassBookStudentNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(
            schoolYear,
            instId,
            classBookId,
            showOnlyWithIndividualCurriculum ?? false,
            showOnlyWithIndividualCurriculumSchedule ?? false,
            ids,
            ct);

    [HttpGet]
    public async Task<ClassBookStudentNomVO[]> GetNomsByTermAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromQuery] bool? showOnlyWithIndividualCurriculum,
        [FromQuery] bool? showOnlyWithIndividualCurriculumSchedule,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IClassBookStudentNomsRepository repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(
            schoolYear,
            instId,
            term,
            classBookId,
            showOnlyWithIndividualCurriculum ?? false,
            showOnlyWithIndividualCurriculumSchedule ?? false,
            offset,
            limit,
            ct);
}
