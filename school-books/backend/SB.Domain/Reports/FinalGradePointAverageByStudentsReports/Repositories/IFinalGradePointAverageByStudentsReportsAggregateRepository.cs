namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IFinalGradePointAverageByStudentsReportsAggregateRepository : IScopedAggregateRepository<FinalGradePointAverageByStudentsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct);
}
