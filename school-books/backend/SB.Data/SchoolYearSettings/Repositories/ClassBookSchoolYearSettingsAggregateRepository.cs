namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class ClassBookSchoolYearSettingsAggregateRepository : ScopedAggregateRepository<ClassBookSchoolYearSettings>, IClassBookSchoolYearSettingsAggregateRepository
{
    public ClassBookSchoolYearSettingsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<ClassBookSchoolYearSettings[]> FindAllByInstitutionAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from cbsys in this.DbContext.Set<ClassBookSchoolYearSettings>()

            join cb in this.DbContext.Set<ClassBook>()
            on new { cbsys.SchoolYear, cbsys.ClassBookId }
            equals new { cb.SchoolYear, cb.ClassBookId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select cbsys
        ).ToArrayAsync(ct);
    }
}
