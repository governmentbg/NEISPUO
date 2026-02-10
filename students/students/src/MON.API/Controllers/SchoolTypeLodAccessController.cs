using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models.SchoolTypeLodAccess;
using MON.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class SchoolTypeLodAccessController : BaseApiController
    {
        private readonly ISchoolTypeLodAccessService _service;

        public SchoolTypeLodAccessController(ISchoolTypeLodAccessService service,
            ILogger<SchoolTypeLodAccessController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchoolTypeLodAccessModel>>> GetAllAsync()
        {
            _logger.LogInformation($"Getting all SchoolTypeLodAccess data.");

            try
            {
                IEnumerable<SchoolTypeLodAccessModel> schoolTypeLodAccessList = await _service.GetAllAsync();
                return Ok(schoolTypeLodAccessList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting all SchoolTypeLodAccess data.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<SchoolTypeLodAccessModel>> GetByIdAsync(int detailedSchoolTypeId)
        {
            _logger.LogInformation($"Getting SchoolTypeLodAccessModel for Id:{detailedSchoolTypeId}");
            try
            {
                SchoolTypeLodAccessModel schoolTypeLodAccessModel = await _service.GetByIdAsync(detailedSchoolTypeId);
                return Ok(schoolTypeLodAccessModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting SchoolTypeLodAccessModel by id:{detailedSchoolTypeId}");

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(SchoolTypeLodAccessModel model)
        {
            _logger.LogInformation($"Updating SchoolTypeLodAccess: {model}");
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
                await _service.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while updating SchoolTypeLodAccessModel for model:{model}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }
    }
}
