namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SelfGovernmentController : BaseApiController
    {
        private readonly IStudentSelfGovernmentService _service;

        public SelfGovernmentController(IStudentSelfGovernmentService selfGovernmentService)
        {
            _service = selfGovernmentService;
        }

        [HttpGet]
        public async Task<SelfGovernmentModel> GetById(int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet]
        public async Task<List<SelfGovernmentViewModel>> GetByPersonId(int personId)
        {
            return await _service.GetByPersonId(personId);
        }

        [HttpPost]
        public async Task Create(SelfGovernmentModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(SelfGovernmentModel model)
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
