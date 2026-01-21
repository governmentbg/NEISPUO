namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IInstitutionsQueryRepository
{
    public Task<TableResultVO<GetAllVO>> GetAllAsync(
        int? regionId,
        string? institutionId,
        string? institutionName,
        string? townName,
        int? offset,
        int? limit,
        CancellationToken ct);

    public Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        bool hasInstitutionAdminWriteAccess,
        CancellationToken ct);

    public Task<GetProtocolTemplateDataVO> GetProtocolTemplateDataAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);
}
