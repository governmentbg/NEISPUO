namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.Diploma.Import;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DiplomaImportValidationExclusionController : BaseApiController
    {
        private readonly IDiplomaImportValidationExclusionService _service;

        public DiplomaImportValidationExclusionController(IDiplomaImportValidationExclusionService service,
            ILogger<DiplomaImportValidationExclusionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<DiplomaImportValidationExclusionModel>> List()
        {
            return await _service.List();
        }

        [HttpPost]
        public async Task AddOrUpdate(DiplomaImportValidationExclusionModel model)
        {
            await _service.AddOrUpdate(model);
        }

        [HttpDelete]
        public async Task Delete([FromQuery]string id)
        {
            await _service.Delete(id);
        }
    }
}
