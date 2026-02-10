using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Services.Interfaces;
using MON.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class AdmissionDocumentController : BaseApiController
    {
        private readonly IAdmissionDocumentService _service;
        public AdmissionDocumentController(IAdmissionDocumentService service, ILogger<AdmissionDocumentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            AdmissionDocumentViewModel result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IEnumerable<AdmissionDocumentViewModel>> GetByPersonId(int personId)
        {
            return await _service.GetByPersonId(personId);
        }

        [HttpGet]
        public async Task<IActionResult> CheckForExistingAdmissionDocumentAsync(int personId, int institutionId)
        {
            bool result = await _service.CheckForExistingAdmissionDocumentAsync(personId, institutionId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> CheckForAdmissionDocumentInTheSameInstitutionAsync(int personId, int institutionId)
        {
            bool result = await _service.CheckForAdmissionDocumentInTheSameInstitutionAsync(personId, institutionId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdmissionDocumentModel model)
        {
            await _service.Create(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(AdmissionDocumentModel model)
        {
            await _service.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task Confirm(int id)
        {
            await _service.Confirm(id);
        }

        [HttpDelete]
        public async Task Delete(int documentId)
        {
            await _service.Delete(documentId).ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<List<AdmissionDocumentViewModel>> GetListForRelocationDocument(int relocationDocumentId)
        {
            return await _service.GetListForRelocationDocument(relocationDocumentId);
        }
    }
}
