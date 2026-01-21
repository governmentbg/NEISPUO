using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Class;
using MON.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class StudentClassController : BaseApiController
    {
        private readonly IStudentClassService _service;
        public StudentClassController(IStudentClassService studentClassService, ILogger<StudentClassController> logger)
        {
            _service = studentClassService;
            _logger = logger;

        }

        [HttpGet]
        public async Task<StudentClassViewModel> GetById(int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet]
        public async Task<List<StudentClassViewModel>> GetHistoryById(int id)
        {
            return await _service.GetHistoryById(id);
        }

        [HttpPost]
        public async Task<List<StudentClassViewModel>> GetByPersonId(StudentClassesTimelineInputModel searchModel)
        {
            return await _service.GetByPersonId(searchModel);
        }

        public async Task<List<StudentClassViewModel>> GetMainForPersonAndLoggedInstitution(int personId, short schoolYear)
        {
            return await _service.GetMainForPersonAndLoggedInstitution(personId, schoolYear);
        }

        public async Task<List<ClassGroupDropdownViewModel>> GetDropdownOptionsForLoggedInstitution(int personId, short? schoolYear)
        {
            return await _service.GetDropdownOptionsForLoggedInstitution(personId, schoolYear);
        }

        [HttpGet]
        public async Task<List<PersonBasicStudentClassDetails>> GetPersonBasicClasses(int personId, bool? forCurrentInstitution)
        {
            return await _service.GetPersonBasicClasses(personId, forCurrentInstitution);
        }

        [HttpGet]
        public async Task<ActionResult<StudentClassSummaryModel>> GetCurrentClassSummaryById(int studentClassId)
        {
            try
            {
                StudentClassSummaryModel studentClassSummary = await _service.GetCurrentClassSummaryByIdAsync(studentClassId);
                return Ok(studentClassSummary);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpPost]
        public async Task EnrollInClass(StudentClassModel model)
        {
            bool withStudentEduForm = true;
            await _service.EnrollInClass(model, withStudentEduForm);
        }

        [HttpPost]
        public async Task EnrollInCplrClass(StudentClassModel model)
        {
            await _service.EnrollInCplrClass(model);
        }

        [HttpPost]
        public async Task EnrollInAdditionalClass(StudentClassBaseModel model)
        {
            await _service.EnrollInAdditionalClass(model);
        }

        [HttpPost]
        public async Task EnrollInCplrAdditionalClass(StudentClassModel model)
        {
            await _service.EnrollInCplrAdditionalClass(model);
        }

        [HttpPut]
        public async Task UpdateAdditionalClass(StudentAdditionalClassChangeModel model)
        {
            await _service.UpdateAdditionalClass(model);
        }

        [HttpPost]
        public async Task<int> ChangeAdditionalClass(StudentAdditionalClassChangeModel model)
        {
            return await _service.ChangeAdditionalClass(model);
        }

        [HttpPut]
        public async Task Update(StudentClassModel model)
        {
            await _service.Update(model);
        }

        [HttpPost]
        public async Task<int> ChangeClassInInstitution(StudentClassModel model)
        {
            bool withStudentEduForm = true;
            return await _service.ChangeStudentClass(model, withStudentEduForm);
        }

        [HttpDelete]
        public async Task DeleteHistoryRecord(int id)
        {
            await _service.DeleteHistoryRecord(id);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }

        [HttpGet]
        public async Task<bool> AddToNewClassBtnVisibilityCheck(int personId)
        {
            return await _service.AddToNewClassBtnVisibilityCheck(personId);
        }

        [HttpPut]
        public async Task UnenrollFromClass(StudentClassUnenrollmentModel model)
        {
            await _service.UnenrollFromClass(model);
        }

        [HttpPut]
        public async Task UnenrollSelected(StudentClassMassUnenrollmentModel model)
        {
            await _service.UnenrollSelected(model);
        }

        [HttpPost]
        public async Task EnrollSelected(StudentClassMassEnrollmentModel model)
        {
            await _service.EnrollSelected(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDualFormCompanies(int studentClassId, CancellationToken cancellationToken)
        {
            return Ok(await _service.GetDualFormCompanies(studentClassId, cancellationToken));
        }

        [HttpPut]
        public async Task ChangePosition(StudentPositionChangeModel model)
        {
            await _service.ChangePosition(model);
        }
    }
}
