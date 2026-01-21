namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IOffDaysAggregateRepository : IScopedAggregateRepository<OffDay>
{
    Task<OffDay[]> FindAllByClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);
}
