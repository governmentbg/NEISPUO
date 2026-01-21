namespace RegStamps.Services.Validation
{
    public interface IValidationService
    {
        Task<(bool isValid, string message)> ValidateActiveRequestExistAsync(int schoolId, int stampId);
    }
}
