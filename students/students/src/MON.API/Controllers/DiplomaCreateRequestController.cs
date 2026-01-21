namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class DiplomaCreateRequestController : BaseApiController
    {
        private readonly IDiplomaCreateRequestService _service;
        public DiplomaCreateRequestController(IDiplomaCreateRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<DiplomaCreateRequestViewModel>> List([FromQuery] PagedListInput input)
        {
            return await _service.List(input);
        }

        [HttpGet]
        public async Task<DiplomaCreateRequestModel> GetById(int id)
        {
            return await _service.GetById(id);
        }

        [HttpPost]
        public async Task Create(DiplomaCreateRequestModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(DiplomaCreateRequestModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }
    }
}
