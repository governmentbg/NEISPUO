namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public class StudentSanctionsController : BaseApiController
    {
        private readonly IStudentSanctionService _service;

        public StudentSanctionsController(IStudentSanctionService studentSanctionService,
            ILogger<StudentSanctionsController> logger)
        {
            _service = studentSanctionService;
            _logger = logger;
        }

        [HttpGet]
        public Task<StudentSanctionModel> GetById(int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]

        public async Task<IPagedList<StudentSanctionViewModel>> List([FromQuery] SanctionsListInput input)
        {
            return await _service.List(input);
        }

        [HttpPost]
        public async Task Create(StudentSanctionModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(StudentSanctionModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }

        [HttpPut]
        public async Task Import(int personId)
        {
            await _service.Import(personId);
        }
    }
}
