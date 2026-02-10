namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IGradeResultsQueryRepository
{
    Task<GetAllVO[]> GetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetAllEditVO[]> GetAllEditAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<bool> HasRemovedFilledSessionAsync(
        int schoolYear,
        int classBookId,
        (int personId, int[] retakeExamCurriculumIds)[] classGradeResults,
        CancellationToken ct);

    Task<GetSessionAllVO[]> GetSessionAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetSessionAllEditVO[]> GetSessionAllEditAsync(
       int schoolYear,
       int classBookId,
       CancellationToken ct);
}
