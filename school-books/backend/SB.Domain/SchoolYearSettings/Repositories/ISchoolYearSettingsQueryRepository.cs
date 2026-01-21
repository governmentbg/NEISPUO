namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface ISchoolYearSettingsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int schoolYearSettingsId,
        CancellationToken ct);

    Task<GetAllForRebuildVO[]> GetAllForRebuildAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<bool> ExistsIsForAllClassesAsync(
        int schoolYear,
        int instId,
        int? exceptSchoolYearSettingsId,
        CancellationToken ct);

    Task<GetDefaultVO> GetDefaultAsync(
        int schoolYear,
        CancellationToken ct);

    Task<bool> ExistsSchoolYearSettingsDefaultAsync(
        int schoolYear,
        CancellationToken ct);

    public Task<bool> IsSportSchoolAsync(int schoolYear, int instId, CancellationToken ct);

    public Task<bool> IsCplrAsync(int schoolYear, int instId, CancellationToken ct);

    public Task RemoveSchoolYearSettingsLinkAsync(
        int schoolYear,
        int instId,
        int schoolYearSettingsId,
        CancellationToken ct);
}
