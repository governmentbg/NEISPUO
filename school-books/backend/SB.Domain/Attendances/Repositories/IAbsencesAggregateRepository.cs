namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IAttendancesAggregateRepository : IScopedAggregateRepository<Attendance>
{
    Task<Attendance[]> FindAllUnexcusedForPeriodForInstWithoutExtProviderAsync(
        int[] schoolYears,
        int personId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken ct);
}
