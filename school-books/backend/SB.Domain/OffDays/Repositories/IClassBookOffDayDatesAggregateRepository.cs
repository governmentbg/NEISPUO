namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookOffDayDatesAggregateRepository : IRepository
{
    Task<ClassBookOffDayDate[]> FindAllByInstitutionAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<ClassBookOffDayDate[]> FindAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);


    Task AddAsync(ClassBookOffDayDate entity, bool preventDetectChanges = false, CancellationToken ct = default);

    void Remove(ClassBookOffDayDate entity);

    void Remove(ClassBookOffDayDate entity, bool forceDetectChangesBeforeRemove = false, bool preventDetectChanges = false);
}
