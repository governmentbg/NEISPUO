namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IOffDaysQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int offDayId,
        CancellationToken ct);

    Task<GetAllForRebuildVO[]> GetAllForRebuildAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<GetAllBasicClassNamesVO[]> GetAllBasicClassNamesAsync(CancellationToken ct);

    Task<GetAllClassBookNamesVO[]> GetAllClassBookNamesAsync(int instId, CancellationToken ct);

    Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<bool> ExistOffDayForAllClassesAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        DateTime from,
        DateTime to,
        CancellationToken ct);

    Task<bool> ExistOffDayForClassesAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        DateTime from,
        DateTime to,
        int[] basicClassIds,
        CancellationToken ct);

    Task<bool> ExistOffDayForClassBooksAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        DateTime from,
        DateTime to,
        int[] classBookIds,
        CancellationToken ct);

    Task<bool> HasHoursInUseAsync(
        int schoolYear,
        int instId,
        DateTime from,
        DateTime to,
        bool isForAllClasses,
        int[] basicClassIds,
        int[] classBookIds,
        CancellationToken ct);
}
