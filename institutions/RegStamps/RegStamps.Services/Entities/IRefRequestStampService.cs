namespace RegStamps.Services.Entities
{
    using Models.Shared.Database;
    using Models.Stamp.ListStamp.Database;
    using Models.Stamp.RequestsForStamp.Database;
    using Models.Stamp.EditStamp.Response;
    using System;

    public interface IRefRequestStampService
    {
        Task<IEnumerable<RequestBackDatabaseModel>> GetReturnBackRequestStamps(int schoolId);

        Task<IEnumerable<RequestStampDatabaseModel>> GetRequestsForStamp(int schoolId, int stampId);

        Task<RequestStampDetailsDatabaseModel> GetRequestStampDetailsAsync(int schoolId, int stampId);

        Task<RequestStampDetailsDatabaseModel> GetRequestDetailsAsync(int schoolId, int stampId, int requestId);

        Task<EditRequestResponseModel> GetRequestDataAsync(int schoolId, int stampId, int requestId);
        
        Task<bool> IsExistAsync(int schoolId, int requestId);
        Task<bool> IsExistAsync(int schoolId, int stampId, bool isActive);
        Task<int> UpdateOrderNumberAsync(int requestId, string orderNumber);
        Task<int> UpdateOrderDateAsync(int requestId, DateTime orderDate);
        Task<int> UpdateStartDateAsync(int requestId, DateTime startDate);
        Task<int> UpdateEndDateAsync(int requestId, DateTime endDate);
        Task<bool> IsExistActiveRequestAsync(int schoolId, int stampId);
    }
}
