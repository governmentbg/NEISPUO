namespace RegStamps.Services.Entities
{
    using Models.Shared.Database;
    using Models.AutoSave.Response;
    using Microsoft.AspNetCore.Http;

    public interface ITbRequestFileService
    {
        Task<IEnumerable<RequestFileDataDatabaseModel>> GetRequestFilesAsync(int schoolId, int requestId);

        Task<IEnumerable<RequestFileResponseModel>> GetRequestUploadFilesAsync(int schoolId, int requestId);

        Task<bool> IsExistAsync(int schoolId, int requestId, int fileId);

        Task<int> InsertRequestFileAsync(int schoolId, int requestId, int fileTypeId, IFormFile file);

        Task<int> DeleteRequestFileAsync(int schoolId, int requestId, int fileId);
    }
}
