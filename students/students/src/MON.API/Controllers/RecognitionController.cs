namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class RecognitionController : BaseApiController
    {
        private readonly IRecognitionService _service;

        public RecognitionController(IRecognitionService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<RecognitionModel> GetById([FromQuery] int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<List<RecognitionViewModel>> GetListForPerson(int personId)
        {
            return _service.GetListForPerson(personId);
        }

        [HttpPost]
        public Task Create(RecognitionModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(RecognitionModel model)
        {
            return _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }

        [HttpGet]
        public Task<string> GetRecognitionRequiredSubjects()
        {
            return _service.GetRecognitionRequiredSubjects();
        }
    }
}
