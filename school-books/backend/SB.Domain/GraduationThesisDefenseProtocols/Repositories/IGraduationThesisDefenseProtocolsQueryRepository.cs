namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IGraduationThesisDefenseProtocolQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int graduationThesisDefenseProtocolId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int graduationThesisDefenseProtocolId,
       CancellationToken ct);
}
