namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.Dashboards;
    using MON.Models.Institution;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ClassGroupController : BaseApiController
    {
        private readonly IClassGroupService _service;
        public ClassGroupController(IClassGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ClassGroupViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<List<StudentInfoExternalModel>> GetStudents(int classId, CancellationToken cancellationToken)
        {
            return await _service.GetStudents(classId, cancellationToken);
        }

        [HttpGet]
        public async Task<List<StudentForAdmissionModel>> GetStudentsForEnrollment(int classId, CancellationToken cancellationToken)
        {
            return await _service.GetStudentsForEnrollment(classId, cancellationToken);
        }

        [HttpGet]
        public async Task<List<StudentForAdmissionModel>> GetStudentsForMassEnrollment(int classId, CancellationToken cancellationToken)
        {
            return await _service.GetStudentsForMassEnrollment(classId, cancellationToken);
        }
    }
}
