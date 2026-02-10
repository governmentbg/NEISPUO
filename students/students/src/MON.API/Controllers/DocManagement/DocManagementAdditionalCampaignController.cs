namespace MON.API.Controllers.DocManagement
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.DocManagement;
    using MON.Models;
    using MON.Services.Implementations;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using MON.Models.Grid;

    public class DocManagementAdditionalCampaignController : BaseApiController
    {
        private readonly DocManagementAdditionalCampaignService _service;

        public DocManagementAdditionalCampaignController(DocManagementAdditionalCampaignService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<DocManagementCampaignViewModel>> List([FromQuery] DocManagementApplicationsListInput input, CancellationToken cancellationToken)
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

        
    }
}
