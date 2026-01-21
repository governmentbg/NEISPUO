namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface INvoExamDutyProtocolsQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int examDutyProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int examDutyProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int examDutyProtocolId,
        int[] personIds,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int examDutyProtocolId,
       CancellationToken ct);
}
