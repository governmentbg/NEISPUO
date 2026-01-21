namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class SchoolYearSettingsAggregateRepository : ScopedAggregateRepository<SchoolYearSettings>, ISchoolYearSettingsAggregateRepository
{
    public SchoolYearSettingsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<SchoolYearSettings>, IQueryable<SchoolYearSettings>>[] Includes =>
        new Func<IQueryable<SchoolYearSettings>, IQueryable<SchoolYearSettings>>[]
        {
            (q) => q.Include(e => e.Classes),
            (q) => q.Include(e => e.ClassBooks)
        };

    public async Task<SchoolYearSettings[]> FindAllByClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(
            sys =>
                sys.SchoolYear == schoolYear &&
                sys.InstId == instId &&
                sys.ClassBooks.Any(syscb =>
                    syscb.SchoolYear == schoolYear &&
                    syscb.ClassBookId == classBookId),
            ct);
    }
}
