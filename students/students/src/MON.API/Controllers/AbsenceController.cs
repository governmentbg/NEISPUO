namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AbsenceController : BaseApiController
    {
        private readonly IAbsenceService _service;

        public AbsenceController(IAbsenceService absenceService)
        {
            _service = absenceService;
        }

        [HttpGet]
        public async Task<IPagedList<StudentAbsenceViewModel>> GetStudentAbsences([FromQuery] StudentListInput input)
        {
            return await _service.GetStudentAbsences(input);
        }

        [HttpGet]
        public async Task<List<StudentAbsenceHistoryOutputModel>> GetStudentAbsencesHistory(int absenceId)
        {
            return await _service.GetStudentAbsencesHistoryAsync(absenceId);
        }

        [HttpGet]
        public async Task<ClassAbsenceModel> GetAbsencesForClass(int classId, short schoolYear, short month)
        {
            return await _service.GetAbsencesForClassAsync(classId, schoolYear, month);
        }

        [HttpGet]
        public async Task<IPagedList<AbsenceImportFileModel>> ListImportedFiles([FromQuery] InstitutionListInput input)
        {
            return await _service.ListImportedFiles(input);
        }

        [HttpPost]
        public async Task Create(StudentAbsenceInputModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(StudentAbsenceInputModel model)
        {
            await _service.Update(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImportAbsencesFromSchoolBooks(short schoolYear, short month)
        {
            await _service.ImportAbsencesFromSchoolBooksAsync(schoolYear, month);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ImportAbsencesFromManualEntry(short schoolYear, short month)
        {
            await _service.ImportAbsencesFromManualEntryAsync(schoolYear, month);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ImportAbsences([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid || file == null || file.Length == 0)
            {
                string errorStr = GetValidationErrors();
                return BadRequest(new
                {
                    message = "Invalid model validation",
                    errorStr
                });
            }

            await _service.ImportAbsencesAsync(file);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ReadAbsenceFile([FromForm] IFormFile file)
        {
            return Ok(await _service.ReadAbsenceFile(file));
        }

        [HttpGet]
        public async Task<AbsenceImportDetailsModel> GetImportDetails(int absenceImportId)
        {
            return await _service.GetImportDetails(absenceImportId);
        }

        [HttpGet]
        public async Task<IPagedList<InstitutionImportedAbsencesOutputModel>> GetInstitutionsWithImportedData([FromQuery] InstitutionsAbsencesListInput input)
        {
            return await _service.GetInstitutionsWithImportedData(input);
        }

        [HttpGet]
        public async Task<IPagedList<InstitutionImportedAbsencesOutputModel>> GetInstitutionsWithoutImportedData([FromQuery] InstitutionsAbsencesListInput input)
        {
            return await _service.GetInstitutionsWithoutImportedData(input);
        }

        [HttpGet]
        public Task<IPagedList<AbsenceExportFileModel>> ListExportedFiles([FromQuery] PagedListInput input)
        {
            return _service.ListExportedFiles(input);
        }

        [HttpPost]
        public async Task<IActionResult> ExportAbsencesToFile(short schoolYear, short month)
        {
            await _service.ExportAbsencesToFileAsync(schoolYear, month);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CopyAspAsking(short schoolYear, short month)
        {
            await _service.CopyAspAsking(schoolYear, month);

            return Ok();
        }

        [HttpDelete]
        public async Task DeleteAbsenceImport(int id)
        {
            await _service.DeleteAbsenceImport(id);
        }

        [HttpGet]
        public async Task<List<AbsenceImportModel>> GetManualImportSampleData(short schoolYear, short month)
        {
            return await _service.GetManualImportSampleData(schoolYear, month);
        }

        [HttpPost]
        public async Task<string> ConstructAbsenceImportAsXml(AbsenceImportDetailsModel model)
        {
            return await _service.ConstructAbsenceImportAsXml(model);
        }

        [HttpPost]
        public async Task<string> ConstructAbsenceExportAsXml(AbsenceExportFileModel model)
        {
            return await _service.ConstructAbsenceExportAsXml(model);
        }

        [HttpGet]
        public async Task<string> ConstructNoAbsencesImportAsXml(int? absenceImportId)
        {
            return await _service.ConstructNoAbsencesImportAsXml(absenceImportId);
        }

        [HttpPut]
        public async Task SetAbsenceImportSigningAtrributes(AbsenceImportSigningAtrributesSetModel model)
        {
            await _service.SetAbsenceImportSigningAtrributes(model);
        }

        [HttpPut]
        public async Task SetAbsenceExportSigningAtrributes(AbsenceExportSigningAtrributesSetModel model)
        {
            await _service.SetAbsenceExportSigningAtrributes(model);
        }

        [HttpPost]
        public Task<int> CreateNoAbsencesImport(NoAbsencesImportModel model)
        {
            return _service.CreateNoAbsencesImport(model);
        }
    }
}
