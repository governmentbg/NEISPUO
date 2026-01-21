namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EnvironmentCharacteristicsController : BaseApiController
    {
        private readonly IEnvironmentCharacteristicsService _environmentCharacteristicsService;

        public EnvironmentCharacteristicsController(ILogger<EnvironmentCharacteristicsController> logger, IEnvironmentCharacteristicsService environmentCharacteristicsService)
        {
            _logger = logger;
            _environmentCharacteristicsService = environmentCharacteristicsService;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRelative(StudentRelativeModel model)
        {
            await _environmentCharacteristicsService.UpdateRelativeAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddRelative(StudentRelativeModel model)
        {
            await _environmentCharacteristicsService.AddRelativeAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEnvironmentCharacteristicsAsync(StudentEnvironmentCharacteristicsModel model)
        {
            _logger.LogInformation($"Updating environment characteristics :{model}");

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
                await _environmentCharacteristicsService.UpdateEnvironmentCharacteristicsAsync(model);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"An error has occured while updating environment characteristics for model:{model}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while updating environment characteristics for model:{model}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<IEnumerable<StudentRelativeModel>> GetRelatives(int personId)
        {
            return await _environmentCharacteristicsService.GetRelativesAsync(personId);
        }

        [HttpGet]
        public async Task<StudentRelativeModel> GetRelative(int relativeId, int personId)
        {
            return await _environmentCharacteristicsService.GetRelativeAsync(relativeId, personId);
        }

        [HttpGet]
        public Task<StudentEnvironmentCharacteristicsModel> GetStudentEnvironmentCharacteristics(int personId)
        {
            return _environmentCharacteristicsService.GetStudentEnvironmentCharacteristics(personId);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRelative(int relativeId, int personId)
        {
            await _environmentCharacteristicsService.DeleteRelativeAsync(relativeId, personId);
            return Ok();
        }

        [HttpGet]
        public async Task<StudentRelativeModel> GetRelatedRelativeByPin(string pin, int personId)
        {
            return await _environmentCharacteristicsService.GetRelatedRelativeByPinAsync(pin, personId);
        }
    }
}
