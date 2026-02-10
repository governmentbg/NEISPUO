namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IAdditionalActivitiesQueryRepository
{
    Task<GetAllAdditionalActivitiesForWeekVO[]> GetAllAdditionalActivitiesForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int additionalActivityId,
        CancellationToken ct);
}
