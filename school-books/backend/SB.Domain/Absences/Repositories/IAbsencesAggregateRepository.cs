namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IAbsencesAggregateRepository : IScopedAggregateRepository<Absence>
{
    Task<Absence[]> FindAllUnexcusedForPeriodForInstWithoutExtProviderAsync(
        int[] schoolYears,
        int personId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken ct);

    Task<Absence[]> FindAllByIdsAsync(
        int schoolYears,
        int[] anbsenceIds,
        CancellationToken ct);
}
