namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IPersonMedicalNoticeAggregateRepository : IRepository
{
    Task<PersonMedicalNotice> FindAsync(int schoolYear, int personId, int hisMedicalNoticeId, CancellationToken ct);

    Task AddAsync(PersonMedicalNotice entity, CancellationToken ct);
}
