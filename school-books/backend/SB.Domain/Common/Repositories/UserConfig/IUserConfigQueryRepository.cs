namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IUserConfigQueryRepository
{
    Task<GetVO> GetAsync(
        int? tokenInstId,
        CancellationToken ct);
}
