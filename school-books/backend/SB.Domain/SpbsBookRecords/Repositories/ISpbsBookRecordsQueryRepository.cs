namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface ISpbsBookRecordsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? recordSchoolYear,
        int? recordNumber,
        string? studentName,
        string? personalId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int spbsBookRecordId,
        CancellationToken ct);

    Task<TableResultVO<GetEscapeVO>> GetEscapeAllAsync(
        int schoolYear,
        int spbsBookRecordId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetEscapeVO> GetEscapeAsync(
        int schoolYear,
        int spbsBookRecordId,
        int orderNum,
        CancellationToken ct);

    Task<TableResultVO<GetAbsenceVO>> GetAbsenceAllAsync(
        int schoolYear,
        int spbsBookRecordId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetAbsenceVO> GetAbsenceAsync(
        int schoolYear,
        int spbsBookRecordId,
        int orderNum,
        CancellationToken ct);
}
