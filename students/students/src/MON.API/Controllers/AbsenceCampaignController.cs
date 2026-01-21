using Microsoft.AspNetCore.Mvc;
using MON.ASPDataAccess;
using MON.Models;
using MON.Models.Absence;
using MON.Models.ASP;
using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class AbsenceCampaignController : BaseApiController
    {
        private readonly IAbsenceCampaignService _service;

        public AbsenceCampaignController(IAbsenceCampaignService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<AbsenceCampaignViewModel>> List([FromQuery] PagedListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<AbsenceReportViewModel>> AbsenceReportsList([FromQuery] AbsenceReportListInput input, CancellationToken cancellationToken)
        {
            return await _service.AbsenceReportsList(input, cancellationToken);
        }

        [HttpGet]
        public async Task<AbsenceCampaignInputModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<AbsenceCampaignViewModel> GetDetailsById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetDetailsById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<AspSessionInfoViewModel> GetAspSession(short schoolYear, short month, string infoType, CancellationToken cancellationToken)
        {
            return await _service.GetAspSession(schoolYear, month, infoType, cancellationToken);
        }

        [HttpPost]
        public async Task Create(AbsenceCampaignInputModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(AbsenceCampaignInputModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }

        [HttpPut]
        public async Task ToggleManuallyActivation(AbsenceCampaignInputModel model)
        {
            await _service.ToggleManuallyActivation(model);
        }

        [HttpGet]
        public async Task<List<AbsenceCampaignViewModel>> GetActive(CancellationToken cancellationToken)
        {
            return await _service.GetActive(cancellationToken);
        }

        [HttpGet]
        public async Task<List<KeyValuePair<string, int>>> GetStats(int id)
        {
            return await _service.GetStats(id);
        }

        [HttpGet("{campaignId}")]
        public async Task<IPagedList<AspStudentGridItemModel>> GetASPStudentsForCampaign(int campaignId, [FromQuery] PagedListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetASPStudentsForCampaign(campaignId, input, cancellationToken);
        }

        [HttpGet("{campaignId}")]
        public async Task<IPagedList<AspMonAbsenceViewModel>> GetAspAbsencesForCampaign(int campaignId, [FromQuery] PagedListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetAspAbsencesForCampaign(campaignId, input, cancellationToken);
        }


        [HttpGet("{campaignId}")]
        public async Task<IPagedList<AspMonZpViewModel>> GetAspZpForCampaign(int campaignId, [FromQuery] AspMonZpListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetAspZpForCampaign(campaignId, input, cancellationToken);
        }
    }
}
