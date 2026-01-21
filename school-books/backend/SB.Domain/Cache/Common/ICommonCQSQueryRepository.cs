namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface ICommonCQSQueryRepository
{
    Task<GetAllExtProvidersVO[]> GetAllExtProvidersAsync(int schoolYear, CancellationToken ct);

    Task<bool> GetInstitutionSchoolYearIsFinalizedAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<bool> ShouldSendNotificationAsync(
        int personId,
        StudentSettingsNotificationType notificationType,
        CancellationToken ct);
}
