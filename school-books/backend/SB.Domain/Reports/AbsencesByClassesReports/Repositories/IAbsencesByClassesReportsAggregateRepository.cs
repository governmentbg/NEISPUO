namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IAbsencesByClassesReportsAggregateRepository : IScopedAggregateRepository<AbsencesByClassesReport>
{
    Task RemoveAsync(
        int schoolYear,
        int absencesByClassesReportId,
        CancellationToken ct);
}
