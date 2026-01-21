namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IExamResultProtocolsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? schoolYear,
        string? orderNumber,
        ExamResultProtocolType? protocolType,
        string? protocolNumber,
        DateTime? protocolDate,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int examResultProtocolId,
        CancellationToken ct);

    Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int examResultProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int examResultProtocolId,
        int[] personIds,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int examResultProtocolId,
       CancellationToken ct);
}
