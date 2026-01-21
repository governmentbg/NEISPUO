namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IExtApiAuthQueryRepository;

public interface IExtApiAuthCachedQueryStore
{
    Task<GetExtSystemVO?> GetExtSystemAsync(string thumbprint, CancellationToken ct);
}
