namespace MonProjects.Services.Logs
{
    using System.Threading.Tasks;

    public class LogService : ILogService
    {
        private const string StoreProcedureCreateLogData = "[ext].[CreateLogData]";

        private readonly IMsSqlNeispuoService neispuoService;

        public LogService(IMsSqlNeispuoService neispuoService)
            => this.neispuoService = neispuoService;

        public async Task<int> CreateLogAsync(string certValue)
            => await this.neispuoService.ExecuteFirstAsync<int>(StoreProcedureCreateLogData,
                new
                {
                    @CertValue = certValue
                });
    }
}
