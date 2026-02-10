namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Update;

    using MON.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ResourceSupportController : BaseApiController
    {
        private readonly IResourceSupportService _service;

        public ResourceSupportController(ILogger<ResourceSupportController> logger,
            IResourceSupportService resourceSupportService)
        {
            _logger = logger;
            _service = resourceSupportService;
        }

        [HttpGet]
        public Task<StudentResourceSupportModel> GetById(int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<List<StudentResourceSupportViewModel>> GetByPersonId(int personId)
        {
            return _service.GetByPersonId(personId);
        }

        [HttpPost]
        public Task Create(StudentResourceSupportModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(StudentResourceSupportModel model)
        {
            return _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }

        [HttpGet]
        public Task<StudentResourceSupportViewModel> ChechForExistingByPerson(int personId, int schoolYear)
        {
            return _service.ChechForExistingByPerson(personId, schoolYear);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudentResourceSupportAsync(StudentResourceSupportUpdateModel model)
        {
            _logger.LogInformation($"Updating resource support :{model}");

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

            try
            {
                await _service.UpdateStudentResourceSupport(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while updating resource support for model:{model}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<ResourceSupportViewModel> GetStudentResourceSupport(int personId, int? schoolYear)
        {
            return _service.GetStudentResourceSupport(personId, schoolYear);
        }
    }
}
