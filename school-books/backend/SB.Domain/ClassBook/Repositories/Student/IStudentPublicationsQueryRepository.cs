namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IStudentPublicationsQueryRepository
{
    Task<TableResultVO<GetStudentPublicationsVO>> GetStudentPublicationsAsync(
        int schoolYear,
        int instId,
        bool archived,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetStudentPublicationsMetadataVO> GetStudentPublicationsMetadataAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);

    Task<GetStudentPublicationVO> GetStudentPublicationAsync(
       int schoolYear,
       int instId,
       int publicationId,
       CancellationToken ct);
}
