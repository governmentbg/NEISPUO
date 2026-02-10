namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IStudentSettingsQueryRepository
{
    Task<GetVO?> GetStudentSettings(
        int personId,
        CancellationToken ct);
}
