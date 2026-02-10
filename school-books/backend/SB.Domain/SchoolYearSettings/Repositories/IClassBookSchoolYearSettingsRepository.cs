namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookSchoolYearSettingsRepository : IRepository
{
    Task<bool> ExistsClassBookSchoolYearSettingsAsync(
        int schoolYear,
        int schoolYearSettingsId,
        CancellationToken ct);
}
