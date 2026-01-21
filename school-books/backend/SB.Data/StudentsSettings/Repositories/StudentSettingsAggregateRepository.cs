namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class StudentSettingsAggregateRepository : ScopedAggregateRepository<StudentSettings>, IStudentSettingsAggregateRepository
{
    public StudentSettingsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<StudentSettings?> FindOrDefaultAsync(int userPersonId, CancellationToken ct)
    {
        return await this.DbContext
            .Set<StudentSettings>()
            .FirstOrDefaultAsync(ss => ss.PersonId == userPersonId, ct);
    }
}
