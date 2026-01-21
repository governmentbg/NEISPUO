namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IQualificationExamResultProtocolsQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int qualificationExamResultProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int qualificationExamResultProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int qualificationExamResultProtocolId,
        int[] personIds,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int qualificationExamResultProtocolId,
       CancellationToken ct);
}
