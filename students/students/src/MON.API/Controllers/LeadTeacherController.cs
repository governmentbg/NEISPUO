using Microsoft.AspNetCore.Mvc;
using MON.ASPDataAccess;
using MON.Models;
using MON.Models.Absence;
using MON.Models.ASP;
using MON.Models.Grid;
using MON.Models.LeadTeacher;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class LeadTeacherController : BaseApiController
    {
        private readonly ILeadTeacherService _service;

        public LeadTeacherController(ILeadTeacherService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IPagedList<LeadTeacherViewModel>> List([FromQuery] PagedListInput input)
        {
            return _service.List(input);
        }

        [HttpGet]
        public Task<LeadTeacherViewModel> GetById(int id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        public Task<LeadTeacherViewModel> GetByClassId(int id)
        {
            return _service.GetByClassId(id);
        }

        [HttpPut]
        public Task Update(LeadTeacherModel model)
        {
            return _service.Update(model);
        }

        [HttpDelete]
        public Task Delete(int classId)
        {
            return _service.Delete(classId);
        }

    }
}
