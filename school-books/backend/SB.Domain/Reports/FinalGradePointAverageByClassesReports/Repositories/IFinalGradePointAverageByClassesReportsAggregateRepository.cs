namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IFinalGradePointAverageByClassesReportsAggregateRepository : IScopedAggregateRepository<FinalGradePointAverageByClassesReport>
{
    Task RemoveAsync(
        int schoolYear,
        int finalGradePointAverageByClassesReportId,
        CancellationToken ct);
}
