namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IMissingTopicsReportsAggregateRepository : IScopedAggregateRepository<MissingTopicsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int missingTopicsReportId,
        CancellationToken ct);
}
