namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IRegularGradePointAverageByClassesReportsAggregateRepository : IScopedAggregateRepository<RegularGradePointAverageByClassesReport>
{
    Task RemoveAsync(
        int schoolYear,
        int regularGradePointAverageByClassesReportId,
        CancellationToken ct);
}
