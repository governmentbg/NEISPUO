namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface ICommonCachedQueryStore
{
    Task<bool> GetInstHasCBExtProviderAsync(int schoolYear, int instId, CancellationToken ct);

    Task<bool> GetExtSystemIsInstCBExtProviderAsync(int extSystemId, int schoolYear, int instId, CancellationToken ct);

    Task<bool> GetExtSystemIsInstScheduleExtProviderAsync(int extSystemId, int schoolYear, int instId, CancellationToken ct);

    Task<bool> GetSchoolYearIsFinalizedAsync(int schoolYear, int instId, CancellationToken ct);

    Task<bool> ShouldSendNotificationAsync(int personId, StudentSettingsNotificationType notificationType, CancellationToken ct);
}
