using MON.Models;
using MON.Models.Certificate;
using MON.Shared.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface ICertificateService
    {
        Task<CertificateModel> GetByIdAsync(int id);
        Task<IPagedList<CertificateModel>> GetCertificatesAsync(PagedListInput input);
        Task UpdateCertificateAsync(CertificateModel model);
        Task CreateCertificateAsync(CertificateModel model);
        Task DeleteCertificateAsync(int id);
        Task<CertificateValidationResultModel> VerifyCertificate(X509Certificate2 cert);
        Task<CertificateValidationResultModel> VerifyXml(string xml);
        Task<CertificateValidationResultModel> VerifyCertificateWithInstitution(X509Certificate2 cert, int? institutionId);
        Task<CertificateValidationResultModel> VerifyXmlWithInstitution(string xml, int? institutionId);
    }
}
