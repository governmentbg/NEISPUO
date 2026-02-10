using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class DocumentController : BaseApiController
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;

        }
        /// <summary>
        /// Извлича документ от хранилището.
        /// TODO - няма security
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FileContentResult> GetById(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            return File(document.NoteContents, document.NoteFileType, document.NoteFileName);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteResourceSupportDocumentByIdAsync(int documentId)
        {
            _logger.LogInformation($"Deleting resource support document for Id:{documentId}");

            try
            {
                await _documentService.DeleteResourceSupportDocumentByIdAsync(documentId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while deleting resource support document for Id:{documentId}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<DocumentViewModel>> TestFileManager()
        {
            return _documentService.TestFileManager();
        }

        [HttpPost]
        public IActionResult TestPostFileManager(List<DocumentModel> models)
        {
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> CheckForExistingAdmissionDocumentAsync(int personId, int institutionId)
        {
            _logger.LogInformation($"Check if there is another admission document from different institution with status Draft for personId: {personId}");

            try
            {
                bool result = await _documentService.CheckForExistingAdmissionDocumentAsync(personId, institutionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while checking if there is another admission document from different institution with status Draft for personId: {personId}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckForAdmissionDocumentInTheSameInstitutionAsync(int personId, int institutionId)
        {
            _logger.LogInformation($"Check if there is another admission document from the same institution with status Confirmed for personId: {personId}");

            try
            {
                bool result = await _documentService.CheckForAdmissionDocumentInTheSameInstitutionAsync(personId, institutionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while checking if there is another admission document from the same institution with status Confirmed for personId: {personId}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }
    }
}
