namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IQualificationAcquisitionProtocolsQueryRepository
{
    Task<GetVO> GetAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        int classId,
        int personId,
        CancellationToken ct);

    Task<bool> IsStudentDuplicatedAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        int classId,
        int personId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int qualificationAcquisitionProtocolId,
       CancellationToken ct);
}
