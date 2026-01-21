namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class ClassBookSchoolYearSettingsRepository : Repository, IClassBookSchoolYearSettingsRepository
{
    public ClassBookSchoolYearSettingsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<bool> ExistsClassBookSchoolYearSettingsAsync(
        int schoolYear,
        int schoolYearSettingsId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ClassBookSchoolYearSettings>()
            .AnyAsync(
                cbsys => cbsys.SchoolYear == schoolYear &&
                    cbsys.SchoolYearSettingsId == schoolYearSettingsId,
                ct);
    }
}
