namespace MON.Services.Interfaces
{
    using MON.Models;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBasicDocumentMarginService
    {
        Task<int> AddOrUpdate(BasicDocumentMarginModel model);
        Task<BasicDocumentMarginModel> Get(int? institutionId, int? regionId, int basicDocumentId, string reportForm, CancellationToken cancellationToken);
    }
}
