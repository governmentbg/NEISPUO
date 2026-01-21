namespace RegStamps.Services.Entities
{
    using Models.Shared.Database;
    using Models.Stamp.EditStamp.Response;

    public interface ITbKeeperService
    {
        Task<KeeperDataDatabaseModel> GetKeeperDetailsAsync(int keeperId);
        Task<EditKeeperResponseModel> GetKeeperDataAsync(int keeperId);
        Task<bool> IsExistAsync(int keeperId);
        Task<int> UpdateKeeperIdNumberAsync(int keeperId, string idNumber, int idType);
        Task<int> UpdateKeeperFirstNameAsync(int keeperId, string firstName);
        Task<int> UpdateKeeperSecondNameAsync(int keeperId, string secondName);
        Task<int> UpdateKeeperFamilyNameAsync(int keeperId, string familyName);
        Task<int> UpdateKeeperOccupationAsync(int keeperId, int occupationId);
    }
}
