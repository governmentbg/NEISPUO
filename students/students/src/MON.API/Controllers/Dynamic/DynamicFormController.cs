namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Dynamic;
    using MON.Models.Grid;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Authorize]
    public class DynamicFormController : BaseApiController
    {
        private readonly IDynamicFormService _service;

        public DynamicFormController(IDynamicFormService service)
        {
            _service = service;
        }

        [HttpGet]
        public string GetJsonSchema()
        {
            return _service.GetJsonSchema();
        }


        [HttpGet]
        public Task<DynamicEntities> GetEntitiesJsonDescription()
        {
            return _service.GetEntitiesJsonDescription();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetEntityTypesDropdowns()
        {
            return _service.GetEntityTypesDropdowns();
        }

        [HttpGet]
        public async Task<IPagedList<string>> List([FromQuery] DynamicEntitiesListInput input)
        {
            try
            {
                return await _service.List(input);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public Task<List<DynamicGridHeader>> GetGridHeaders(string entityTypeName)
        {
            return _service.GetGridHeaders(entityTypeName);
        }

        [HttpGet]
        public Task<string> GetEntityModel(string entityTypeName, string entityId)
        {
            return _service.GetEntityModel(entityTypeName, entityId);
        }

        [HttpPost]
        public Task Create([FromBody] dynamic model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update([FromBody] dynamic model)
        {
            return _service.Update(model);
        }

        // Използването на post за триене на е грешка. Трябва да подам body, а не се стравих с axios и 415 Status code.
        [HttpPost]
        public Task Delete([FromBody] dynamic model)
        {
            return _service.Delete(model);
        }
    }
}
