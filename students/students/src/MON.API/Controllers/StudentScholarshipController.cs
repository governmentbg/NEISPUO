using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class StudentScholarshipController : BaseApiController
    {
        private readonly IStudentScholarshipService _service;

        public StudentScholarshipController(IStudentScholarshipService studentScholarshipService,
            ILogger<StudentScholarshipController> logger)
        {
            _service = studentScholarshipService;
            _logger = logger;
        }

        [HttpGet]
        public Task<ScholarshipViewModel> GetById(int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<StudentScholarshipsViewModel> GetByPersonId(int personId, int? schoolYear)
        {
            return _service.GetByPersonId(personId, schoolYear);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentScholarshipModel model)
        {
            await _service.Create(model);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(StudentScholarshipModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid model:{model}");

                string errorStr = GetValidationErrors();
                return BadRequest(new
                {
                    message = "Invalid model validation",
                    errorStr
                });
            }

            await _service.Update(model);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);

            return Ok();
        }
    }
}
