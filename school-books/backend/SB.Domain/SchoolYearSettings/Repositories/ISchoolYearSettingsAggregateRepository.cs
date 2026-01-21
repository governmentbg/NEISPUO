namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface ISchoolYearSettingsAggregateRepository : IScopedAggregateRepository<SchoolYearSettings>
{
    Task<SchoolYearSettings[]> FindAllByClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct);
}
