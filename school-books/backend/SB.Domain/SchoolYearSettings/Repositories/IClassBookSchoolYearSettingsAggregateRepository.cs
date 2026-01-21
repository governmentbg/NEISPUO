namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookSchoolYearSettingsAggregateRepository : IScopedAggregateRepository<ClassBookSchoolYearSettings>
{
    Task<ClassBookSchoolYearSettings[]> FindAllByInstitutionAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);
}
