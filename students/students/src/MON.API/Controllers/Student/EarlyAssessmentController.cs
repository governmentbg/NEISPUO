namespace MON.API.Controllers.Student
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System.Threading.Tasks;

    public class EarlyAssessmentController : BaseApiController
    {
        private readonly IEarlyAssessmentService _service;

        public EarlyAssessmentController(IEarlyAssessmentService service)
        {
            _service = service;
        }   

        public async Task<StudentEarlyAssessmentModel> GetByPerson(int personId)
        {
            return await _service.GetByPerson(personId);
        }


        [HttpPost]
        public async Task AddOrUpdate(StudentEarlyAssessmentModel model)
        {
            await _service.CreateOrUpdate(model);
        }
    }
}
