namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Grid;
    using MON.Models.Refugee;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class RefugeeController : BaseApiController
    {
        private readonly IRefugeeService _refugeeService;

        public RefugeeController(IRefugeeService refugeeService)
        {
            _refugeeService = refugeeService;
        }

        [HttpGet]
        public async Task<ApplicationModel> GetById(int id)
        {
            return await _refugeeService.GetByIdAsync(id);
        }

        [HttpGet]
        public async Task<ApplicationViewModel> GetDetailsById(int id)
        {
            return await _refugeeService.GetDetailsByIdAsync(id);
        }

        [HttpGet]
        public async Task<IPagedList<ApplicationViewModel>> ApplicationList([FromQuery] RefugeeApplicationListInput input)
        {
            var result = await _refugeeService.ApplicationList(input);

            return result;
        }

        [HttpGet]
        public async Task<IPagedList<RefugeeAdmissionViewModel>> AdmissionList([FromQuery] PagedListInput input)
        {
            return await _refugeeService.AdmissionList(input);
        }

        [HttpGet]
        public async Task<int> CountPendingAdmissions()
        {
            return await _refugeeService.CountPendingAdmissions();
        }

        [HttpPost]
        public async Task Create(ApplicationModel model)
        {
            await _refugeeService.CreateApplication(model);
        }

        [HttpPut]
        public async Task Update(ApplicationModel model)
        {
            await _refugeeService.UpdateApplication(model);
        }

        [HttpPut]
        public async Task CompleteApplication(int id)
        {
            await _refugeeService.CompleteApplication(id);
        }

        [HttpPut]
        public async Task CompleteApplicationChild(int id)
        {
            await _refugeeService.CompleteApplicationChild(id);
        }

        [HttpDelete]
        public async Task DeleteApplication(int id)
        {
            await _refugeeService.DeleteApplication(id);
        }

        [HttpDelete]
        public async Task DeleteApplicationChild(int id)
        {
            await _refugeeService.DeleteApplicationChild(id);
        }

        [HttpPut]
        public async Task CancelApplication(RefugeeApplicationCancellationModel model)
        {
            await _refugeeService.CancelApplication(model);
        }

        [HttpPut]
        public async Task CancelApplicationChild(RefugeeApplicationCancellationModel model)
        {
            await _refugeeService.CancelApplicationChild(model);
        }

        [HttpPut]
        public async Task UnlockApplication(int id)
        {
            await _refugeeService.UnlockApplication(id);
        }

        [HttpPut]
        public async Task UnlockApplicationChild(int id)
        {
            await _refugeeService.UnlockApplicationChild(id);
        }
    }
}
