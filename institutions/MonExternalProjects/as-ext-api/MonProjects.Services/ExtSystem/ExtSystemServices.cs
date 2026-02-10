namespace MonProjects.Services.ExtSystem
{
    using Services.Models.ExtSystem;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ExtSystemServices : IExtSystemServices
    {
        private const string StoreProcedureGetExtSystemServices = "[ext].[GetServices]";

        private readonly IMsSqlNeispuoService neispuoService;

        public ExtSystemServices(IMsSqlNeispuoService neispuoService)
            => this.neispuoService = neispuoService;
        

        public async Task<IEnumerable<ServiceDataDapperModel>> GetServicesAsync(int extSystemId)
            => await this.neispuoService.ExecuteListAsync<ServiceDataDapperModel>(
                StoreProcedureGetExtSystemServices,
                new
                {
                    @ExtSystemId = extSystemId
                });
    }
}
