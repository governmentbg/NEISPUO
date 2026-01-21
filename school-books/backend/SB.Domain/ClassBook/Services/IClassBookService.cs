namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookService
{
    Task<int[]> CreateClassBooks(
        int schoolYear,
        int instId,
        (int classId, string? classBookName)[] classBooks,
        int sysUserId, CancellationToken ct);
}
