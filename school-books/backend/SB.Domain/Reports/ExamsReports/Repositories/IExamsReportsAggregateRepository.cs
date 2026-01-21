namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IExamsReportsAggregateRepository : IScopedAggregateRepository<ExamsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int examsReportId,
        CancellationToken ct);
}
