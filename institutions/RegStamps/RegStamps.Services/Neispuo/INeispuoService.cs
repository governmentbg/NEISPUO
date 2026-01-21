namespace RegStamps.Services.Neispuo
{
    using Models.Shared.Response;
    using RegStamps.Models.Shared.Database;

    public interface INeispuoService
    {
        Task<IEnumerable<OccupationDatabaseModel>> GetOccupationsAsync();

        Task<SchoolDataResponseModel> GetSchoolInfoAsync(int schoolId);

        Task<IEnumerable<SchoolDataResponseModel>> GetAllSchoolsAsync();
    }
}
