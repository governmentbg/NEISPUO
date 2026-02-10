namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IGradelessStudentsReportsAggregateRepository : IScopedAggregateRepository<GradelessStudentsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int gradelessStudentsReportId,
        CancellationToken ct);
}
