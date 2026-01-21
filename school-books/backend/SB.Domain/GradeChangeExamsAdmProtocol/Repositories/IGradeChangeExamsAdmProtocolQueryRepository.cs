namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IGradeChangeExamsAdmProtocolQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct);

    Task<bool> IsStudentDuplicatedAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
        int schoolYear,
        int admProtocolId,
        CancellationToken ct);
}
