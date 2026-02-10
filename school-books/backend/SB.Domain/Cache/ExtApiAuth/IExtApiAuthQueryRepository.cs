namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IExtApiAuthQueryRepository
{
    Task<GetExtSystemVO?> GetExtSystemAsync(string thumbprint, CancellationToken ct);
}
