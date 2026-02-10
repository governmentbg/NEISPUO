namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IDateAbsencesReportsAggregateRepository : IScopedAggregateRepository<DateAbsencesReport>
{
    Task RemoveAsync(
        int schoolYear,
        int dateAbsencesReportId,
        CancellationToken ct);
}
