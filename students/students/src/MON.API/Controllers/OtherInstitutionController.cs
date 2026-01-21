using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class OtherInstitutionController : BaseApiController
    {
        private readonly IOtherInstitutionService _otherInstitutionService;
        public OtherInstitutionController(IOtherInstitutionService otherInstitutionService, ILogger<RegixController> logger)
        {
            _otherInstitutionService = otherInstitutionService;
            _logger = logger;

        }

        [HttpGet]
        public Task<OtherInstitutionViewModel> GetById(int institutionId)
        {
            return _otherInstitutionService.GetByIdAsync(institutionId);
        }

        [HttpPut]
        public async Task<IActionResult> Update(OtherInstitutionViewModel model)
        {
            await _otherInstitutionService.UpdateAsync(model);
            return Ok();
        }

        [HttpPost]
        public Task Create(OtherInstitutionViewModel model)
        {
            return _otherInstitutionService.CreateAsync(model);
        }

        [HttpGet]
        public Task<List<OtherInstitutionViewModel>> GetStudentOtherInstitutions(int personId)
        {
            return _otherInstitutionService.GetStudentOtherInstitutions(personId);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _otherInstitutionService.DeleteAsync(id);
        }
    }
}
