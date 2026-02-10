namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBooksQueryRepository
{
    Task<GetAllVO[]> GetAllAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<GetAllVO[]> GetAllForTeacherAsync(
        int schoolYear,
        int instId,
        int personId,
        CancellationToken ct);

    Task<GetAllForFinalizationVO[]> GetAllForFinalizationAsync(
        int schoolYear,
        int instId,
        int[]? classBookIds,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetStudentsVO[]> GetStudentsAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<int[]> GetCurriculumIdsForClassBookAsync(
        int schoolYear,
        int classId,
        bool classIsLvl2,
        CancellationToken ct);

    Task<GetRemovedStudentsVO[]> GetRemovedStudentsAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int[] personIds,
        CancellationToken ct);

    Task<GetDefaultCurriculumVO> GetDefaultCurriculumAsync(
        int? personId,
        int schoolYear,
        int classBookId,
        bool excludeGradeless,
        CancellationToken ct);

    Task<GetCurriculumsVO[]> GetCurriculumsAsync(
        int schoolYear,
        int classBookId,
        bool excludeGradeless,
        bool includeInvalidWithGrades,
        bool includeInvalidWithTopicPlans,
        CancellationToken ct);

    Task<DateTime[]> GetReplTeacherDatesAsync(
        int schoolYear,
        int classBookId,
        int personId,
        CancellationToken ct);

    Task<string?> GetClassBookPrintContentHashAsync(
        int schoolYear,
        int classBookId,
        int classBookPrintId,
        CancellationToken ct);

    Task<string[]> GetClassBookNamesByIdsAsync(
        int schoolYear,
        int instId,
        int[] classsBookIds,
        CancellationToken ct);
}
