using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{

    public class DischargeDocumentController : BaseApiController
    {
        private readonly IDischargeDocumentService _service;

        public DischargeDocumentController(IDischargeDocumentService service,
            ILogger<DischargeDocumentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public Task<DischargeDocumentModel> GetById([FromQuery] int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<List<DischargeDocumentModel>> GetListForPerson(int personId)
        {
            return _service.GetByPersonId(personId);
        }

        [HttpPost]
        public Task Create(DischargeDocumentModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(DischargeDocumentModel model)
        {
            return _service.Update(model);
        }

        [HttpPut]
        public Task Confirm(int id)
        {
            return _service.Confirm(id);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
