namespace RegStamps.Services.Entities
{
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.Http;

    using Models.Stamp.ListStamp.Database;
    using Models.Shared.Database;
    using Models.Stamp.EditStamp.Response;

    public interface ITbStampService
    {
        Task<IEnumerable<StampDataDatabaseModel>> GetAllStampsAsync(int schoolId);

        Task<StampDetailsDataDatabaseModel> GetStampDetailsAsync(int schoolId, int stampId);

        Task<EditStampResponseModel> GetStampDataAsync(int schoolId, int stampId);

        Task<bool> IsExistAsync(int stampId);
        
        Task<bool> IsExistAsync(int schoolId, int stampId);

        Task<int> CreateStampIdAsync();

        Task<int> CreateRequestNewStampAsync(int schoolId, int stampId);

        Task<int> CreateRequestChangeStampKeeperAsync(int schoolId, int stampId);

        Task<int> CreateRequestDestroyStampAsync(int schoolId, int stampId);

        Task<int> CreateRequestLostStampAsync(int schoolId, int stampId);

        Task<int> UpdateStampTypeAsync(int schoolId, int stampId, int stampTypeId);

        Task<int> UpdateFirstUseDateAsync(int schoolId, int stampId, DateTime firstUseDate);

        Task<int> UpdateLetterDateAsync(int schoolId, int stampId, DateTime letterDate);

        Task<int> UpdateFirstUsePersonAsync(int schoolId, int stampId, string firstUsePerson);

        Task<int> UpdateLetterNumberAsync(int schoolId, int stampId, string letterNumber);

        Task<int> UpdateStampFileAsync(int schoolId, int stampId, IFormFile file);

        Task<int> DeleteStampFileAsync(int schoolId, int stampId);

        Task<int> DeleteStampAsync(int schoolId, int stampId);

        Task<int> DeleteStampRequestAsync(int schoolId, int stampId, int requestId);

        Task<(int result, string message)> SendStampRequestAsync(int schoolId, int stampId, int requestId, string signData, X509Certificate2 certificate);
    }
}
