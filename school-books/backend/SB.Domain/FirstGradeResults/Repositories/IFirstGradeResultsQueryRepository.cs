namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IFirstGradeResultsQueryRepository
{
    Task<GetAllVO[]> GetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);
}
