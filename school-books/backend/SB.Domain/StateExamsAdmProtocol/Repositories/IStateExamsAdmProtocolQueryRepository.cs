namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IStateExamsAdmProtocolQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? schoolYear,
        AdmProtocolType? protocolType,
        string? protocolNum,
        DateTime? protocolDate,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct);

    Task<bool> IsStudentDuplicatedAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
        int schoolYear,
        int admProtocolId,
        CancellationToken ct);
}
