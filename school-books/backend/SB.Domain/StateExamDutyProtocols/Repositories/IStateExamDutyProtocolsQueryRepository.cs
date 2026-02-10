namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IStateExamDutyProtocolsQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int stateExamDutyProtocolId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int stateExamDutyProtocolId,
       CancellationToken ct);
}
