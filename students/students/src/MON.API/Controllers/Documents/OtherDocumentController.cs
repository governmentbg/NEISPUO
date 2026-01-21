namespace MON.API.Controllers.Documents
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class OtherDocumentController : BaseApiController
    {
        private readonly IOtherDocumentService _service;
        public OtherDocumentController(IOtherDocumentService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<OtherDocumentModel> GetById([FromQuery] int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<List<OtherDocumentModel>> GetListForPerson(int personId)
        {
            return _service.GetByPersonId(personId);
        }

        [HttpPost]
        public Task Create(OtherDocumentModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(OtherDocumentModel model)
        {
            return _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
