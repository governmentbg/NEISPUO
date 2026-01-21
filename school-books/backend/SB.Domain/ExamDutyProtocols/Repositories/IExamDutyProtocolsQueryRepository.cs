namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IExamDutyProtocolsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? schoolYear,
        ExamDutyProtocolType? protocolType,
        string? orderNumber,
        DateTime? orderDate,
        DateTime? protocolDate,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int examDutyProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int examDutyProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int examDutyProtocolId,
        int[] personIds,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int examDutyProtocolId,
       CancellationToken ct);

    Task<int[]> GetStudentPersonIdsByClassIdAsync(
        int schoolYear,
        int classId,
        int[] excludedPersonIds,
        CancellationToken ct);
}
