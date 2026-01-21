namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface ISessionStudentsReportsAggregateRepository : IScopedAggregateRepository<SessionStudentsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int sessionStudentsReportId,
        CancellationToken ct);
}
