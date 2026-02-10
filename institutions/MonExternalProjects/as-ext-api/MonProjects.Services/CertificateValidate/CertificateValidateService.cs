namespace MonProjects.Services.CertificateValidate
{
    using Models.CertificateValidate;
    using System.Threading.Tasks;

    public class CertificateValidateService : ICertificateValidateService
    {
        private const string StoreProcedureGetCertificateData = "[ext].[GetCertificateData]";

        private readonly IMsSqlNeispuoService neispuoService;

        public CertificateValidateService(IMsSqlNeispuoService neispuoService)
            => this.neispuoService = neispuoService;
        

        public async Task<CertificateDataDapperModel> GetCertificateDataAsync(string thumbprint)
            => await this.neispuoService.ExecuteFirstAsync<CertificateDataDapperModel>(
                StoreProcedureGetCertificateData,
                new 
                {
                    @Thumbprint = thumbprint
                });
    }
}
