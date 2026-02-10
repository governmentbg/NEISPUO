namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IStudentMedicalNoticesQueryRepository
{
    Task<GetAllStudentsVO[]> GetAllStudentsAsync(
        int[] studentPersonIds,
        CancellationToken ct);

    Task<string> GetStudentNameAsync(
        int studentPersonId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentMedicalNoticesVO>> GetStudentMedicalNoticesAsync(
        int schoolYear,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct);
}
