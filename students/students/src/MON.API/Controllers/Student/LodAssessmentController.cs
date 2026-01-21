namespace MON.API.Controllers.Student
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Grid;
    using MON.Models.StudentModels.Lod;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LodAssessmentController : BaseApiController
    {
        private readonly ILodAssessmentService _service;

        public LodAssessmentController(ILodAssessmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IPagedList<StudentLodAssessmentListModel>> List([FromQuery] LodEvaluationListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<LodAssessmentImportListModel>> ListImported([FromQuery] LodAssessmentListInput input, CancellationToken cancellationToken)
        {
            return await _service.ListImported(input, cancellationToken);
        }

        [HttpGet]
        public async Task<List<LodAssessmentCurriculumPartModel>> GetPersonAssessments(int personId, int basicClass, int schoolYear, bool isSelfEduForm, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken)
        {
            return await _service.GetPersonAssessments(personId, basicClass, schoolYear, isSelfEduForm, filterForCurrentInstitution, filterForCurrentSchoolBook, cancellationToken);
        }

        [HttpGet]
        public async Task<List<LodAssessmentCurriculumPartModel>> GetStudentClassCurriculum(int studentClassId, CancellationToken cancellationToken)
        {
            return await _service.GetStudentClassCurriculum(studentClassId, cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromForm] IFormFile file)
        {
            await _service.Import(file);
            return Ok();
        }

        [HttpPost]
        public async Task<LodAssessmentImportValidationModel> ValidateImport([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            LodAssessmentImportValidationModel validationModel = await _service.ValidateImport(file, cancellationToken);

            return validationModel;
        }

        [HttpPost]
        public async Task CreateOrUpdate(List<LodAssessmentCurriculumPartModel> model)
        {
            await _service.CreateOrUpdate(model);
        }

        [HttpGet]
        public async Task<List<ClassGroupDropdownViewModel>> GetMainStudentClasses(int personId, CancellationToken cancellationToken)
        {
            return await _service.GetMainStudentClasses(personId, cancellationToken);
        }


        [HttpDelete]
        public async Task Delete(int personId, int basicClass, int schoolYear, bool isSelfEduForm, int institutionId, CancellationToken cancellationToken)
        {
            await _service.Delete(personId, basicClass, schoolYear, isSelfEduForm, institutionId, cancellationToken);
        }

        [HttpPost]
        public IActionResult ExportFile(LodAssessmentImportValidationModel model)
        {
            byte[] fileContents = _service.ExportFile(model);
            return File(fileContents, "text/plain", $"{model.Models.FirstOrDefault()?.InstitutionId:0000000}_{model.Models.FirstOrDefault().SchoolYear:0000_replaced}.txt");
        }
    }
}
