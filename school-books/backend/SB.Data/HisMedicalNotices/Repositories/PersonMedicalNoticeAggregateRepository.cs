namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SB.Data;

class PersonMedicalNoticeAggregateRepository : Repository, IPersonMedicalNoticeAggregateRepository
{
    public PersonMedicalNoticeAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<PersonMedicalNotice> FindAsync(int schoolYear, int personId, int hisMedicalNoticeId, CancellationToken ct)
    {
        return await this.FindEntityAsync(
            this.DbContext.Set<PersonMedicalNotice>(),
            new object[] { schoolYear, personId, hisMedicalNoticeId },
            Array.Empty<Func<IQueryable<PersonMedicalNotice>, IQueryable<PersonMedicalNotice>>>(),
            ct);
    }

    public async Task AddAsync(PersonMedicalNotice entity, CancellationToken ct)
    {
        await this.DbContext.AddAsync(entity, ct);
    }
}
