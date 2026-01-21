namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IPublicationsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int publicationId,
        CancellationToken ct);

    Task<TableResultVO<GetAllPublishedVO>> GetAllPublishedAsync(
        int schoolYear,
        int instId,
        bool archived,
        PublicationType? type,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetMetadataVO> GetMetadataAsync(
        int schoolYear,
        int instId,
        PublicationType? type,
        CancellationToken ct);

    Task<GetPublishedVO> GetPublishedAsync(
       int schoolYear,
       int instId,
       int publicationId,
       CancellationToken ct);
}
