namespace RegStamps.Services.Neispuo
{
    using Infrastructure.Extensions;

    using Models.Shared.Database;
    using Models.Shared.Response;


    public class NeispuoService : INeispuoService
    {
        private readonly IHttpService httpService;

        public NeispuoService(IHttpService httpService)        
            => this.httpService = httpService;

        public async Task<IEnumerable<OccupationDatabaseModel>> GetOccupationsAsync()
        {
            HttpResponseMessage response = await this.httpService.GetAsync("Datas?getREGSTAMPSOccup");

            if (response.IsSuccessStatusCode)
            {
                return (await response.DeserializeToCollectionAsync<OccupationDatabaseModel>())
                        .Where(x => x.OccupId > 0)
                        .OrderBy(x => x.OccupType)
                        .ThenBy(x => x.OccupId);
            }
            else
            {
                throw new Exception(await response.GetErrorMessageAsync());
            }
        }

        public async Task<SchoolDataResponseModel> GetSchoolInfoAsync(int schoolId)
        {
            HttpResponseMessage response = await this.httpService.GetAsync($"Datas?getREGSTAMPSInstitution/{schoolId}");

            if (response.IsSuccessStatusCode)
            {
                return (await response.DeserializeToCollectionAsync<SchoolDataResponseModel>())
                        .FirstOrDefault();
            }
            else
            {
                throw new Exception(await response.GetErrorMessageAsync());
            }
        }

        public async Task<IEnumerable<SchoolDataResponseModel>> GetAllSchoolsAsync()
        {
            HttpResponseMessage response = await this.httpService.GetAsync($"Datas?getREGSTAMPSInstitution");

            if (response.IsSuccessStatusCode)
            {
                return await response.DeserializeToCollectionAsync<SchoolDataResponseModel>();
            }
            else
            {
                throw new Exception(await response.GetErrorMessageAsync());
            }
        }
    }
}
