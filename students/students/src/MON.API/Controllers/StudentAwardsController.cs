namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StudentAwardsController : BaseApiController
    {
        private readonly IStudentAwardService _service;

        public StudentAwardsController(IStudentAwardService studentAwardService, ILogger<StudentAwardsController> logger)
        {
            _service = studentAwardService;
            _logger = logger;
        }

        [HttpGet]
        public Task<StudentAwardModel> GetById(int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<IEnumerable<StudentAwardViewModel>> GetByPersonId(int personId)
        {
            return _service.GetByPersonId(personId);
        }

        [HttpPost]
        public Task Create(StudentAwardModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(StudentAwardModel model)
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
