namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.ExternalEvaluation;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Authorize]
    public class ExternalEvaluationController : BaseApiController
    {
        private readonly IExternalEvaluationService _service;

        public ExternalEvaluationController(IExternalEvaluationService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<List<ExternalEvaluationModel>> GetByPersonId(int personId)
        {
            return _service.GetByPersonId(personId);
        }

        [HttpPost]
        public Task Create(ExternalEvaluationModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(ExternalEvaluationModel model)
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
