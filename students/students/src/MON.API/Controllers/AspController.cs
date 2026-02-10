namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.ASP;
    using MON.Models.Enums;
    using MON.Models.Grid;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AspController : BaseApiController
    {
        private readonly IAspService _service;

        private const string SUPPORTED_FILE_FORMAT = ".txt";

        public AspController(IAspService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPManage)]
        public Task<IPagedList<ASPMonthlyBenefitsImportFileModel>> GetImportedBenefitsFiles([FromQuery] ASPBenefitsInput input)
        {
            return _service.GetImportedBenefitsFilesAsync(input);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPImport)]
        public async Task<IActionResult> ImportBenefits([FromForm] IFormFile file)
        {
            ValidateFileInput(file);

            var result = await _service.ImportBenefitsAsync(file);

            if (result.ImportStarted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPImport)]
        public async Task LoadAspConfirmations(AspSessionInfoViewModel sessionInfoModel)
        {

            await _service.LoadAspConfirmations(sessionInfoModel);
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPImport)]
        public async Task<IPagedList<AspUnprocessedRequestViewModel>> UnprocessedAspConfirmsList([FromQuery] ApPUnprocessedRequestsInput input)
        {
            return await _service.UnprocessedAspConfirmsList(input);
        }


        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPImportDetailsShow)]
        public async Task<IPagedList<ASPMonthlyBenefitModel>> GetImportedBenefitsDetails([FromQuery] ASPBenefitsInput input)
        {
            return await _service.GetImportedBenefitsDetailsAsync(input);
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPImportDetailsShow)]
        public async Task<ASPMonthlyBenefitViewModel> GetImportedBenefitsFileMetaData(int importedFileId)
        {
            return await _service.GetImportedBenefitsFileMetaDataAsync(importedFileId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateImportedBenefitsFileMetaData(ASPMonthlyBenefitsImportFileModel model)
        {
            await _service.UpdateImportedBenefitsFileMetaDataAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBenefit(ASPMonthlyBenefitModel model)
        {
            await _service.UpdateBenefitAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ExportApprovedMonthlyBenefits(int campaignId)
        {
            await _service.ExportApprovedMonthlyBenefits(campaignId);
            return Ok();
        }

        [HttpGet]
        public async Task<IPagedList<ASPMonthlyBenefitReportDetailsDto>> GetInstitutionsStillReviewingBenefits([FromQuery] ASPBenefitsInput input)
        {
            return await _service.GetInstitutionsStillReviewingBenefitsAsync(input);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPEnrolledStudentsExport)]
        public async Task<IActionResult> ExportEnrolledStudents(EnrolledStudentsExportModel model)
        {
            switch (model.FileType)
            {
                case (int)AspEnrolledStudentsExportFileType.EnrolledStudents:
                    await _service.ExportEnrolledStudentsAsync(model);
                    break;
                case (int)AspEnrolledStudentsExportFileType.EnrolledStudentsCorrections:
                    await _service.ExportEnrolledStudentsCorrectionsAsync(model);
                    break;
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPEnrolledStudentsExport)]
        public async Task<IActionResult> CheckForExistingEnrolledStudentsExport(EnrolledStudentsExportModel model)
        {
            bool result = await _service.CheckForExistingEnrolledStudentsExportAsync(model);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForASPEnrolledStudentsExport)]
        public Task<IEnumerable<ASPEnrolledStudentsExportFileModel>> GetEnrolledStudentsExportFiles(short? schoolYear)
        {
            return _service.GetEnrolledStudentsExportFilesAsync(schoolYear);
        }

        [HttpPost]
        public async Task<string> ConstructAspBenefitsAsXml(AspBenefitsModel model)
        {
            return await _service.ConstructAspBenefitsAsXml(model);
        }

        [HttpPut]
        public Task SetAspBenefitsSigningAtrributes(AspBenefitsSigningAtrributesSetModel model)
        {
            return _service.SetAspBenefitsSigningAtrributes(model);
        }

        [HttpPost]
        public Task RemoveAspBenefitsSigningAtrributes(AspBenefitsSigningAtrributesSetModel model)
        {
            return _service.RemoveAspBenefitsSigningAtrributes(model);
        }

        [HttpDelete]
        public Task DeleteImporedFileRecord(int fileId)
        {
            return _service.DeleteImporedFileRecordAsync(fileId);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSubmittedData([FromForm] IFormFile file)
        {
            await _service.UploadSubmittedData(file);
            return Ok();
        }

        [HttpGet]
        public Task<IPagedList<ASPEnrolledStudentSubmittedDataViewModel>> EnrolledStudentsSubmittedDataList([FromQuery] EnrolledStudentsSubmittedDataListInput input)
        {
            return _service.ListEnrolledStudentsSubmittedData(input);
        }

        [HttpGet]
        public Task<List<KeyValuePair<string, int>>> GetCampaignStats(int id)
        {
            return _service.GetCampaignStats(id);
        }

        [HttpGet]
        public Task<MonSessionInfoViewModel> GetMonSession(short schoolYear, short month, string infoType)
        {
            return _service.GetMonSession(schoolYear, month, infoType);
        }

        [HttpGet("{sessionNo}")]
        public Task<IPagedList<AspMonConfirmViewModel>> GetMonConfirmsForCampaign(int sessionNo, [FromQuery] PagedListInput input)
        {
            return _service.GetMonConfirmsForCampaign(sessionNo, input);
        }

        private void ValidateFileInput(IFormFile file)
        {
            if (!file.FileName.EndsWith(SUPPORTED_FILE_FORMAT))
            {
                throw new ArgumentException("Non supported file type.");
            }

            if (!ModelState.IsValid || file == null || file.Length == 0)
            {
                var errorStr = GetValidationErrors();
                throw new ArgumentNullException($"No file found attached: {errorStr}");
            }
        }
    }
}
