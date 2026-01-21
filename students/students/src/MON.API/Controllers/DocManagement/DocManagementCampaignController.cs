namespace MON.API.Controllers.DocManagement
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Implementations;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;
    using MON.Models.DocManagement;
    using System.Threading;
    using System.Collections.Generic;
    using System;

    public class DocManagementCampaignController : BaseApiController
    {
        private readonly DocManagementCampaignService _service;

        public DocManagementCampaignController(DocManagementCampaignService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<DocManagementCampaignViewModel>> List([FromQuery] PagedListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<DocManagementCampaignViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpPost]
        public async Task Create(DocManagementCampaignModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(DocManagementCampaignModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _service.Delete(id, cancellationToken);
        }

        [HttpPut]
        public async Task ToggleManuallyActivation(DocManagementCampaignModel model)
        {
            await _service.ToggleManuallyActivation(model);
        }

        [HttpGet]
        public async Task<List<DocManagementCampaignViewModel>> GetActive(CancellationToken cancellationToken)
        {
            return await _service.GetActive(cancellationToken);
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetDropdownOptions(CancellationToken cancellationToken)
        {
            return await _service.GetDropdownOptions(cancellationToken);
        }

        [HttpPost]
        public async Task<FileContentResult> GenerateReport(DocManagementReportImputModel model, CancellationToken cancellationToken)
        {
            (string fileName, byte[] fileContents, string contentType) = await _service.GenerateReport(model, cancellationToken);
            return File(
                fileContents: fileContents,
                contentType: contentType,
                fileDownloadName: fileName
            );
        }

        [HttpGet]
        public async Task DownloadAllAttachments(int campaignId, CancellationToken cancellationToken)
        {
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"Campaign_{campaignId}_Attachments.zip\"");

            await _service.DownloadAllAttachments(campaignId, Response.Body, cancellationToken);
        }

        [HttpGet]
        public async Task<List<DocumentViewModel>> GetAttachments(int campaignId, CancellationToken cancellationToken)
        {
            return await _service.GetAttachments(campaignId, cancellationToken);
        }

        [HttpPost]
        public async Task SaveAttachments(DocManagementCampaignAttachmentModel model)
        {
            await _service.SaveAttachments(model);
        }
    }
}
