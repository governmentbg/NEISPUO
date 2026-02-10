
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Models.Diploma;
using MON.Models.Grid;
using MON.Services.Interfaces;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class DiplomaController : BaseApiController
    {
        private readonly IDiplomaService _diplomaService;
        private readonly IExcelService _excelService;

        public DiplomaController(IDiplomaService diplomaService,
            IExcelService excelService,
            ILogger<DiplomaController> logger)
        {
            _diplomaService = diplomaService;
            _excelService = excelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IPagedList<DiplomaViewModel>> List([FromQuery] DiplomaListInput input)
        {
            return await _diplomaService.List(input);
        }

        [HttpGet]
        public async Task<IPagedList<VStudentDiploma>> ListUnimported([FromQuery] DiplomaListInput input)
        {
            return await _diplomaService.ListUnimported(input);
        }

        [HttpGet]
        public async Task<string> ConstructDiplomaByIdAsXml(int id)
        {
            // Временно спираме валидацията на дипломата, докато изчистим схемите и направим по-добри съобщения
            //var validationResult = await _diplomaService.Validate(id);
            //if (validationResult.HasErrors)
            //{
            //    throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            //}
            return await _diplomaService.ConstructDiplomaByIdAsXmlAsync(id);
        }

        [HttpGet]
        public async Task<DiplomaSigningData> GetDiplomaSigningData(int id)
        {
            return await _diplomaService.GetDiplomaSigningDataAsync(id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaDocumentModel>>> GetDiplomaDocuments(int diplomaId)
        {
            var diplomDocuments = await _diplomaService.GetDiplomaDocumentsAsync(diplomaId);
            return Ok(diplomDocuments);
        }

        [HttpPost]
        public async Task<ResultModel<int>> UploadDiplomaDocument([FromForm] IFormFile file, [FromForm] int diplomaId, [FromForm] string description, [FromForm] DocumentModel scannedFile, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileContents = ms.ToArray();
            var result = await _diplomaService.UploadDiplomaDocumentAsync(fileContents, diplomaId, description, file.FileName, file.ContentType, cancellationToken);
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDiplomaImage(DiplomaDocumentDTOModel model, CancellationToken cancellationToken)
        {
            await _diplomaService.UploadDiplomaDocumentAsync(Convert.FromBase64String(model.Document.Base64Contents), model.DiplomaId, model.Description, model.Document.NoteFileName, model.Document.NoteFileType, cancellationToken);
            return Ok();
        }

        [HttpGet]

        public Task<DiplomaBasicDetailsModel> GetBasicDetails(int diplomaId)
        {
            return _diplomaService.GetBasicDetails(diplomaId);
        }

        [HttpGet]
        public async Task<ActionResult> RemoveDiplomaDocument(int id)
        {
            await _diplomaService.RemoveDiplomaDocumentAsync(id);
            return StatusCode(204);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDiplomaFinalizationStepsAsync(DiplomaFinalizationUpdateModel model)
        {
            await _diplomaService.UpdateDiplomaFinalizationStepsAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<DiplomaFinalizationViewModel>> GetDiplomaFinalizationDetailsByIdAsync(int diplomaId)
        {
            DiplomaFinalizationViewModel model = await _diplomaService.GetDiplomaFinalizationDetailsByIdAsync(diplomaId);
            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult> ReorderDiplomaDocuments(DiplomaOrderDocuments model)
        {
            await _diplomaService.ReorderDiplomaDocumentsAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> AnullDiploma(DiplomaAnullationModel model)
        {
            await _diplomaService.AnnulDiploma(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            await _diplomaService.Import(file, cancellationToken);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SetAsEditable(DiplomaSetAsEditableModel model)
        {
            await _diplomaService.SetAsEditable(model);
            return Ok();
        }

        [HttpGet]
        public async Task<DiplomaCreateModel> GetCreateModel(int? personId, int? templateId, int? basicDocumentId, int? basicClassId, CancellationToken cancellationToken)
        {
            return await _diplomaService.GetCreateModel(personId, templateId, basicDocumentId, basicClassId, cancellationToken);
        }

        [HttpGet]
        public async Task<DiplomaCreateModel> GetUpdateModel(int diplomaId)
        {
            return await _diplomaService.GetUpdateModel(diplomaId);
        }

        [HttpGet]
        public async Task<IEnumerable<DiplomaAdditionalDocumentViewModel>> GetOriginalDocuments(int? personId, string? personalId, int? personalIdType, [FromQuery]int[] mainBasicDocuments, CancellationToken cancellationToken)
        {
            return await _diplomaService.GetOriginalDocuments(personId, personalId, personalIdType, mainBasicDocuments, cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiplomaCreateModel model)
        {
            ApiValidationResult validationResult = await _diplomaService.Create(model);
            return Ok(validationResult);
        }

        [HttpPut]
        public async Task<IActionResult> Update(DiplomaUpdateModel model)
        {
            ApiValidationResult validationResult = await _diplomaService.Update(model);
            return Ok(validationResult);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _diplomaService.Delete(id);
            return Ok();
        }

        [HttpGet]
        public async Task<FileContentResult> GenerateApplicationFile(int diplomaId)
        {
            var fileContents = await _diplomaService.GenerateApplicationFileAsync(diplomaId);
            return File(fileContents, "application/docx", $"{diplomaId}.docx");
        }

        [HttpGet]
        public async Task<DiplomaViewModel> GetAdditionalDocumentDetails(int personId, int basicDocumentId)
        {
            return await this._diplomaService.GetAdditionalDocumentDetails(personId, basicDocumentId);
        }

        [HttpGet]
        public async Task<IList<DropdownViewModel>> GetRegBookBasicDocuments(string searchStr, RegBookTypeEnum regBookType)
        {
            return await _diplomaService.GetRegBookBasicDocuments(regBookType);
        }

        [HttpGet]
        public async Task<IPagedList<RegBookDetailsModel>> RegBookList([FromQuery] RegBookListInput input)
        {
            return await _diplomaService.GetRegBookList(input);
        }

        [HttpGet]
        public async Task<FileContentResult> GenerateRegBookExport([FromQuery] RegBookListInput input)
        {
            var regBookList = await (await _diplomaService.GetRegBookListAll(input)).ToListAsync();

            var excelData = _excelService.DumpExcel<RegBookDetailsModel>(regBookList, $"{input.Year}");
            return File(excelData, "application/vnd.ms-excel", $"Рег. книга.xlsx");
        }
    }
}
