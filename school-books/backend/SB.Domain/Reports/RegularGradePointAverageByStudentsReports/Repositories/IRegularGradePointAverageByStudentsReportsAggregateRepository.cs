namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IRegularGradePointAverageByStudentsReportsAggregateRepository : IScopedAggregateRepository<RegularGradePointAverageByStudentsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int regularGradePointAverageByStudentsReportId,
        CancellationToken ct);
}
