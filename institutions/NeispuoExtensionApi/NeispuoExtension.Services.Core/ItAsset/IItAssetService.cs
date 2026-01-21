namespace NeispuoExtension.Services.Core.ItAsset
{
    using System.Threading.Tasks;

    using DependencyInjection;

    using Models.ItAsset.Request;
    using Models.ItAsset.Response;

    public interface IItAssetService : ITransientService
    {
        public Task<InsertItAssetFromExcelResponseModel> InsertFromExcelAsync(InsertItAssetFromExcelRequestModel request);
    }
}
