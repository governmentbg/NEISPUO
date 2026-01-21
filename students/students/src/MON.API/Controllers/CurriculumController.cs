namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.StudentModels.Class;
    using MON.Services.Implementations;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class CurriculumController : BaseApiController
    {
        private readonly CurriculumService _service;

        public CurriculumController(CurriculumService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<List<CurriculumClassModel>> GetForStudentClass(int studentClassId, int? status, CancellationToken cancellationToken)
        {
            return _service.GetForStudentClass(studentClassId, status, cancellationToken);
        }

        [HttpPut]
        public Task AddForStudentClass(CurriculumStudentUpdateModel model)
        {
            return _service.AddSelectedForStudentClass(model);
        }

        [HttpPut]
        public Task EditCurriculumStudent(CurriculumStudentUpdateModel model)
        {
            return _service.EditCurriculumStudent(model);
        }

        [HttpPut]
        public Task RemoveForStudentClass(CurriculumStudentUpdateModel model)
        {
            return _service.DeleteForStudentClass(model, false);
        }
    }
}
