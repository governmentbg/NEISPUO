namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IHighSchoolCertificateProtocolQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int highSchoolCertificateProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int highSchoolCertificateProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int highSchoolCertificateProtocolId,
        int[] personIds,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
        int schoolYear,
        int admProtocolId,
        CancellationToken ct);
}
