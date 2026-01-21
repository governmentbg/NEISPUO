namespace MonProjects.Services.CertificateValidate
{
    using Services.Models.CertificateValidate;
    using System.Threading.Tasks;

    public interface ICertificateValidateService
    {
        Task<CertificateDataDapperModel> GetCertificateDataAsync(string thumbprint);
    }
}
