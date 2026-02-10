namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IHisMedicalNoticesQueryRepository
{
    Task<MedicalNoticeBatchDO> GetNextWithReadReceiptsAndSaveAsync(
        int extSystemId,
        int next,
        CancellationToken ct);

    Task<NeispuoMedicalNoticeBatchVO> GetNeispuoNextWithReadReceiptsAndSaveAsync(
        int next,
        CancellationToken ct);

    Task ExecuteAcknowledgeAsync(int extSystemId, int[] hisMedicalNoticeIds, CancellationToken ct);

    Task<int> ExecuteAcknowledgeNeispuoAsync(int hisMedicalNoticeId, CancellationToken ct);

    public Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        string? nrnMedicalNotice,
        string? nrnExamination,
        string? identifier,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<TableResultVO<GetAllVO>> GetAllForRegion(
        int regionId,
        int schoolYear,
        string? nrnMedicalNotice,
        string? nrnExamination,
        string? identifier,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<GetVO> GetAsync(
        int hisMedicalNoticeId,
        CancellationToken ct);
}
