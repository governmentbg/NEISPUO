namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class DiplomaTemplateController : BaseApiController
    {
        private readonly IDiplomaTemplateService _service;

        public DiplomaTemplateController(IDiplomaTemplateService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<DiplomaTemplateListModel>> List([FromQuery] PagedListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<BasicDocumentTemplateModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<BasicDocumentTemplateModel> GetForBasicDocument(int basicDocumentId, CancellationToken cancellationToken)
        {
            return await _service.GetForBasicDocument(basicDocumentId, cancellationToken);
        }

        [HttpPost]
        public async Task Create(BasicDocumentTemplateModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(BasicDocumentTemplateModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _service.Delete(id, cancellationToken);
        }

        [HttpGet]
        public async Task<List<DiplomaTemplateDropdownViewModel>> DropdownOptions(string searchStr, int? basicDocumentId, CancellationToken cancellationToken)
        {
            return await _service.GetDropdownOptions(searchStr, basicDocumentId, cancellationToken);
        }
    }
}
