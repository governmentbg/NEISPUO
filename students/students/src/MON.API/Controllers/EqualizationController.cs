namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EqualizationController : BaseApiController
    {
        private readonly IEqualizationService _service;

        public EqualizationController(IEqualizationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<EqualizationModel> GetById([FromQuery] int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet]
        public async Task<List<EqualizationGridViewModel>> GetListForPerson(int personId)
        {
            return await _service.GetListForPerson(personId);
        }

        [HttpPost]
        public async Task Create(EqualizationModel model)
        {
            await _service.Create(model);
        }
        
        [HttpPut]
        public async Task Update(EqualizationModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetEqualizationReasonTypeClasses()
        {
            return await _service.GetEqualizationReasonTypeClasses();
        }
    }
}
