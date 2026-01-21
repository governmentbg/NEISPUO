namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface ISkillsCheckExamResultProtocolsQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int skillsCheckExamResultProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetEvaluatorAllVO>> GetEvaluatorAllAsync(
        int schoolYear,
        int skillsCheckExamResultProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetEvaluatorVO> GetEvaluatorAsync(
        int schoolYear,
        int skillsCheckExamResultProtocolId,
        int SkillsCheckExamResultProtocolEvaluatorId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int skillsCheckExamResultProtocolId,
       CancellationToken ct);
}
