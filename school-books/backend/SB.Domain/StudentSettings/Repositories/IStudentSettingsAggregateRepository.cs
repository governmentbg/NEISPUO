namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IStudentSettingsAggregateRepository : IScopedAggregateRepository<StudentSettings>
{
    Task<StudentSettings?> FindOrDefaultAsync(int userPersonId, CancellationToken ct);
}
