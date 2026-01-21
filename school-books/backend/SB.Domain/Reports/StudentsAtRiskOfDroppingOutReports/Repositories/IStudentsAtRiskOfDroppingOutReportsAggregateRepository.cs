namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IStudentsAtRiskOfDroppingOutReportAggregateRepository : IScopedAggregateRepository<StudentsAtRiskOfDroppingOutReport>
{
    Task RemoveAsync(
        int schoolYear,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct);
}
