namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StudentPersonalDevelopmentSupportController : BaseApiController
    {
        private readonly IStudentPersonalDevelopmentSupportService _service;

        public StudentPersonalDevelopmentSupportController(IStudentPersonalDevelopmentSupportService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<PersonalDevelopmentSupportModel> GetById([FromQuery] int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<List<PersonalDevelopmentSupportViewModel>> GetListForPerson(int personId)
        {
            return _service.GetListForPerson(personId);
        }

        [HttpPost]
        public Task Create(PersonalDevelopmentSupportModel model)
        {
            return _service.Create(model);
        }

        [HttpPut]
        public Task Update(PersonalDevelopmentSupportModel model)
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
