namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IRemarksQueryRepository
{
    public Task<GetAllForClassBookVO[]> GetAllForClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    public Task<GetAllForStudentAndTypeVO[]> GetAllForStudentAndTypeAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        RemarkType type,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int remarkId,
        CancellationToken ct);
}
