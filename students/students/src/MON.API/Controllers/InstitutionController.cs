namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Grid;
    using MON.Models.Institution;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InstitutionController : BaseApiController
    {
        private readonly IInstitutionService _service;

        public InstitutionController(IInstitutionService institutionService)
        {
            _service = institutionService;
        }

        [HttpGet]
        public Task<InstitutionCacheModel> GetById(int? institutionId)
        {
            return _service.GetByIdAsync(institutionId);
        }

        [HttpGet]
        public async Task<IPagedList<InstitutionDetailsModel>> List([FromQuery] InstitutionListInput input)
        {
            return await _service.List(input);
        }

        [HttpGet]
        public Task<IPagedList<StudentInfoExternalModel>> ExternalDetailsList([FromQuery] InstitutionListInput input)
        {
            return _service.ListInstitutionExternalDetails(input);
        }

        [HttpGet]
        public Task<IPagedList<StudentInfoFullModel>> StudentsList([FromQuery] InstitutionListInput input)
        {
            return _service.ListStudents(input);
        }

        [HttpGet]
        public Task<short> GetCurrentYear(int? institutionId)
        {
            return _service.GetCurrentYear(institutionId);
        }

        [HttpGet]
        public Task<InstitutionDropdownViewModel> GetDropdownModelById(int institutionId)
        {
            return _service.GetDropdownModelByIdAsync(institutionId);
        }

        [HttpGet]
        public Task<IPagedList<ClassGroupBaseModel>> GetClassGroups([FromQuery] ClassesListInput input)
        {
            return _service.GetClassGroupsForInstitutionAsync(input);
        }

        [HttpGet]
        public Task<List<ClassGroupDropdownViewModel>> GetClassGroupsForAdditionalEnrollment(int personId, short schoolYear, string selectedValues)
        {
            return _service.GetClassGroupsForAdditionalEnrollment(personId, schoolYear, selectedValues);
        }

        [HttpGet]
        public Task<List<InstitutionInfoModel>> GetInstitutions()
        {
            return _service.GetInstitutionsAsync();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetInstitutionsDropdownModel()
        {
            return _service.GetInstitutionsDropdownModelAsync();
        }

        [HttpGet]
        public async Task<ActionResult> AutoNumber(int classId)
        {
            await _service.AutoNumberAsync(classId);
            return StatusCode(204);
        }

        [HttpGet]
        public Task<int?> GetCurrentForStudent(int? studentId)
        {
            return _service.GetCurrentForStudent(studentId);
        }

        [HttpGet]
        public Task<InstitutionDetailsModel> GetFullDetails(int? id)
        {
            return _service.GetFullDetails(id);
        }

        [HttpGet]
        public Task<InstitutionDropdownViewModel> GetLoggedUserInstitution()
        {
            return _service.GetLoggedUserInstitution();
        }

        [HttpGet]
        public Task<bool> HasExternalSoProviderForLoggedInstitution()
        {
            return _service.HasExternalSoProviderForLoggedInstitution();
        }
    }
}
