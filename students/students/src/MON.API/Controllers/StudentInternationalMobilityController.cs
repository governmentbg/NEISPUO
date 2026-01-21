namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StudentInternationalMobilityController : BaseApiController
    {
        private readonly IStudentInternationalMobilityService _studentInternationalMobilityService;

        public StudentInternationalMobilityController(IStudentInternationalMobilityService studentInternationalMobilityService, ILogger<StudentInternationalMobilityController> logger)
        {
            _studentInternationalMobilityService = studentInternationalMobilityService;
            _logger = logger;
        }

        [HttpGet]
        public Task<InternationalMobilityModel> GetById(int internationalMobilityId)
        {
            return _studentInternationalMobilityService.GetById(internationalMobilityId);
        }

        [HttpGet]
        public Task<List<InternationalMobilityViewModel>> GetByPersonId(int personId)
        {
            return _studentInternationalMobilityService.GetByPersonId(personId);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InternationalMobilityModel model)
        {
            await _studentInternationalMobilityService.Create(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int internationalMobilityId)
        {
            await _studentInternationalMobilityService.Delete(internationalMobilityId);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(InternationalMobilityModel model)
        {
            await _studentInternationalMobilityService.Update(model);
            return Ok();
        }
    }
}
