namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Certificate;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class CertificateController : BaseApiController
    {
        private readonly ICertificateService _service;

        public CertificateController(ICertificateService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<CertificateModel> GetById(int id)
        {
            return _service.GetByIdAsync(id);
        }

        [HttpGet]
        public Task<IPagedList<CertificateModel>> GetCertificates([FromQuery] PagedListInput input)
        {
            return _service.GetCertificatesAsync(input);
        }

        [HttpPost]
        public Task Create(CertificateModel model)
        {
            return _service.CreateCertificateAsync(model);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var certificate = await _service.GetByIdAsync(id);

            return File(certificate.Contents, "application/x-x509-ca-cert", certificate.Name);
        }

        [HttpPut]
        public Task Update(CertificateModel model)
        {
            return _service.UpdateCertificateAsync(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.DeleteCertificateAsync(id);
        }

        [HttpGet] 
        public Task<CertificateValidationResultModel> Verify(string xml)
        {
            return _service.VerifyXml(xml);
        }


    }
}
