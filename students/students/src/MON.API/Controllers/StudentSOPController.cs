namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Update;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class StudentSOPController : BaseApiController
    {
        private readonly IStudentSOPService _service;

        public StudentSOPController(IStudentSOPService studentSOPService,
            ILogger<StudentSOPController> logger)
        {
            _service = studentSOPService;
            _logger = logger;
        }

        [HttpGet]
        public Task<StudentSopUpdateModel> GetById([FromQuery] int id, CancellationToken cancellationToken)
        {
            return _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public Task<List<SopViewModel>> GetListForPerson(int personId, CancellationToken cancellationToken)
        {
            return _service.GetListForPerson(personId, cancellationToken);
        }

        [HttpPost]
        public Task Create(StudentSopUpdateModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(StudentSopUpdateModel model)
        {
            return _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int id, CancellationToken cancellationToken)
        {
            return _service.Delete(id, cancellationToken);
        }
    }
}
