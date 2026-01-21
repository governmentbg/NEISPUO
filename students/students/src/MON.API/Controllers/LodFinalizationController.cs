namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels.Lod;
    using MON.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LodFinalizationController : BaseApiController
    {
        private readonly ILodFinalizationService _service;

        public LodFinalizationController(ILogger<LodFinalizationController> logger,
            ILodFinalizationService lodFinalizationService)
        {
            _logger = logger;
            _service = lodFinalizationService;
        }

        [HttpPost]
        public async Task<IActionResult> ApproveLodAsync(LodFinalizationModel model)
        {
            _logger.LogInformation($"Approving student lod for model:{model}");

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
                await _service.ApproveLodAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while approving student lod for model:{model}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpPost]
        public async Task SignLodAsync(LodSignatureModel model)
        {
            await _service.SignLodAsync(model);
        }

        [HttpPost]
        public async Task SignLodUndoAsync(LodSignatureUndoModel model)
        {
            await _service.SignLodUndoAsync(model);
        }

        [HttpPost]
        public async Task<ActionResult> ApproveLodUndoAsync(LodFinalizationUndoModel model)
        {
            _logger.LogInformation($"Undo approving of student lod for model:{model}");

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
                await _service.ApproveLodUndoAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while undo approving of student lod for model:{model}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<short>> GetStudentFinalizedLods(int personId)
        {
            return _service.GetStudentFinalizedLods(personId);
        }
    }
}
