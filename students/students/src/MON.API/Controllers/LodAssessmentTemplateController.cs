namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;
    using System.Threading;
    using MON.Models;
    using MON.Models.Ores;
    using System.Collections.Generic;

    public class LodAssessmentTemplateController : BaseApiController
    {
        private readonly ILodAssessmentTemplateService _service;

        public LodAssessmentTemplateController(ILodAssessmentTemplateService service)
        {
            _service= service;
        }

        [HttpGet]
        public async Task<IPagedList<LodAssessmentTemplateViewModel>> List([FromQuery] PagedListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<LodAssessmentTemplateViewModel> GetById(int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet]
        public Task<IAsyncEnumerable<DropdownViewModel>> GetDropdownOptions(int? basicClassId)
        {
            return _service.GetDropdownOptions(basicClassId);
        }

        [HttpPost]
        public async Task Create(LodAssessmentTemplateModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(LodAssessmentTemplateModel model)
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
