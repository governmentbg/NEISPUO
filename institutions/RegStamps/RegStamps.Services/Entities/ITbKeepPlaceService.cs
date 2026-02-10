namespace RegStamps.Services.Entities
{
    public interface ITbKeepPlaceService
    {
        Task<bool> IsExistAsync(int keepPlaceId);

        Task<int> UpdateKeepPlaceNameAsync(int keepPlaceId, string keepPlaceName);
    }
}
