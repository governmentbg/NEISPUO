namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IAbsencesByStudentsReportsAggregateRepository : IScopedAggregateRepository<AbsencesByStudentsReport>
{
    Task RemoveAsync(
        int schoolYear,
        int absencesByStudentsReportId,
        CancellationToken ct);
}
