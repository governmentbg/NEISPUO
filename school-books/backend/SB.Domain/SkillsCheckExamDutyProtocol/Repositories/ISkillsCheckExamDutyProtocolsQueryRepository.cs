namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface ISkillsCheckExamDutyProtocolsQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int skillsCheckExamDutyProtocolId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int skillsCheckExamDutyProtocolId,
       CancellationToken ct);
}
