namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface ISchoolYearSettingsRebuildService
{
    Task RebuildAndSaveAsync(
        int schoolYear,
        int instId,
        ITransaction transaction,
        CancellationToken ct);
}
