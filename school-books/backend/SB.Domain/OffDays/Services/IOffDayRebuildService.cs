namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IOffDayRebuildService
{
    Task RebuildAndSaveAsync(
        int schoolYear,
        int instId,
        int? exceptOffDayId,
        ITransaction transaction,
        CancellationToken ct);
    Task<ClassBookOffDayDate[]> CreateForClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? basicClassId,
        CancellationToken ct);
}
