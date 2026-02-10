namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookReorganizeService
{
    Task<int> CombineClassBooks(
        int schoolYear,
        int instId,
        int parentClassId,
        string? parentClassBookName,
        int? childClassIdForDataTransfer,
        int sysUserId, CancellationToken ct);

    public Task<int[]> SeparateClassBooks(
        int schoolYear,
        int instId,
        int parentClassId,
        (int classId, string? classBookName)[] childClassBooks,
        int sysUserId,
        CancellationToken ct);
}
