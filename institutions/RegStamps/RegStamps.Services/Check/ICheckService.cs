namespace RegStamps.Services.Check
{
    using Models.Check.Response;

    public interface ICheckService
    {
        Task<IEnumerable<CheckMessageResponseModel>> CheckRequestDataAsync(int schoolId, int stampId, int requestId, int keeperId, int placeId);
    }
}
