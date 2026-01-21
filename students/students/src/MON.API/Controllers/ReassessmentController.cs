namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ReassessmentController : BaseApiController
    {
        private readonly IReassessmentService _service;

        public ReassessmentController(IReassessmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ReassessmentModel> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        [HttpGet]
        public async Task<List<ReassessmentModel>> GetListForPerson(int personId)
        {
            return await _service.GetListForPersonAsync(personId);
        }

        [HttpPost]
        public async Task Create(ReassessmentModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(ReassessmentModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
