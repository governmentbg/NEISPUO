namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBooksAggregateRepository : IScopedAggregateRepository<ClassBook>
{
    Task<ClassBook?> FindOrDefaultAsync(int schoolYear, int classBookId, CancellationToken ct);
    Task<ClassBook[]> FindAllByIdsAsync(int schoolYear, int[] classBookIds, CancellationToken ct);
    Task<ClassBook[]> FindAllByClassIdsAsync(int schoolYear, int[] classIds, CancellationToken ct);
    Task AddAsync(ClassBookPrint entity, CancellationToken ct);
    Task AddAsync(ClassBookStudentPrint entity, CancellationToken ct);
}
