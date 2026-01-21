using Microsoft.AspNetCore.Mvc;
using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class AdmissionPermissionRequestController : BaseApiController
    {
        private readonly IAdmissionPermissionRequestService _service;

        public AdmissionPermissionRequestController(IAdmissionPermissionRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<AdmissionPermissionRequestModel> GetById(int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet]
        public async Task<IPagedList<AdmissionPermissionRequestViewModel>> List([FromQuery] AdmissionDocRequestListInput input)
        {
            return await _service.List(input);
        }

        [HttpGet]
        public async Task<int> CountPending()
        {
            return await _service.CountPending();
        }

        [HttpPost]
        public async Task Create(AdmissionPermissionRequestModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(AdmissionPermissionRequestModel model)
        {
            await  _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }

        [HttpPut]
        public async Task Confirm(int id)
        {
            await _service.Confirm(id);
        }
    }
}
