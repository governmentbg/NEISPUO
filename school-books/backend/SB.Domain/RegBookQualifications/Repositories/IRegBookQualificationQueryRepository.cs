namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IRegBookQualificationQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        string? registrationNumberTotal,
        string? registrationNumberYear,
        DateTime? registrationDate,
        string? fullName,
        string? identifier,
        int? basicDocumentId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int id,
        CancellationToken ct);

    Task<GetExcelDataVO[]> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int basicDocumentId,
        CancellationToken ct);

    Task<GetWordDataVO> GetWordDataAsync(
        int schoolYear,
        int id,
        CancellationToken ct);
}
