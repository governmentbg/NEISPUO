namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using System;
    using System.Threading.Tasks;

    public class BarcodeYearController : BaseApiController
    {
        private readonly IBarcodeYearService _barcodeYearService;

        public BarcodeYearController(IBarcodeYearService barcodeYearService, ILogger<BarcodeYearController> logger)
        {
            _barcodeYearService = barcodeYearService;
            _logger = logger;
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForDiplomaBarcodesEdit)]
        public async Task<IActionResult> UpdateBarcodeYearAsync(BarcodeYearModel model)
        {
            _logger.LogInformation($"Updating BarcodeYear: {model}");
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
                await _barcodeYearService.UpdateBarcodeYearAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while updating BarcodeYear for model:{model}", ex);

                return BadRequest(new
                {
                    message = GetErrorMessageForUI(ex),
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForDiplomaBarcodesShow)]
        public async Task<ActionResult<BarcodeYearModel>> GetBarcodeYear(int barcodeYearId)
        {
            _logger.LogInformation($"Getting BarcodeYear for barcodeYearId:{barcodeYearId}");

            try
            {
                var barcodeYear = await _barcodeYearService.GetBarcodeYearByIdAsync(barcodeYearId);
                return barcodeYear;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting BarcodeYear for barcodeYearId:{barcodeYearId}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForDiplomaBarcodesShow)]
        public async Task<ActionResult<BarcodeYearListViewModel>> GetBarcodeYears(int basicDocumentId)
        {
            _logger.LogInformation($"Getting barcodeYears for basicDocumentId:{basicDocumentId}");

            try
            {
                var barcodeYears = await _barcodeYearService.GetBarcodeYearsAsync(basicDocumentId);
                return Ok(barcodeYears);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting barcodeYears for basicDocumentId:{basicDocumentId}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBarcodeYearAsync(int id)
        {
            _logger.LogInformation($"Deleting BarcodeYear by Id:{id}");

            try
            {
                await _barcodeYearService.DeleteBarcodeYearAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while deleting BarcodeYear for Id:{id}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForDiplomaBarcodesAdd)]
        public async Task<IActionResult> AddBarcodeYearAsync(BarcodeYearModel model)
        {
            _logger.LogInformation($"Adding BarcodeYear for BasicDocumentId:{model.BasicDocumentId}");

            try
            {
                await _barcodeYearService.AddBarcodeYearAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while adding BarcodeYear for BasicDocumentId:{model.BasicDocumentId}", ex);
                return BadRequest(new
                {
                    message = GetErrorMessageForUI(ex),
                    statusCode = 500
                });
            }
        }

        private string GetErrorMessageForUI(Exception ex)
        {
            string errorMessage;

            if (ex.InnerException.Message.Contains("UQ_BarcodeYear_Edition_BasicDocumentId_SchoolYear"))
            {
                errorMessage = "errors.diplomaBarcodeEditionSchoolYearNotUniqueMessage";
            }
            else
            {
                errorMessage = "diplomas.diplomaCreateErrorMsg";
            }

            return errorMessage;
        }
    }
}
