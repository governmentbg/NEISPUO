namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models.Grid;
    using MON.Models.Institution;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Update;
    using MON.Models.StudentModels.View;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MON.Shared;
    using System.Threading;
    using MON.Models.NoteModels;
    using MON.Models.StudentModels.Lod;
    using System.Linq;

    public class StudentLODController : BaseApiController
    {
        private readonly IStudentLODService _studentLODService;

        public StudentLODController(ILogger<StudentLODController> logger, IStudentLODService studentLODService)
        {
            _logger = logger;
            _studentLODService = studentLODService;
        }

        [HttpGet]
        public Task<IPagedList<VStudentGeneralTrainingDatum>> GeneralTrainingDataList([FromQuery] StudentGeneralTrainingDataListInput input)
        {
            return _studentLODService.GetGeneralTrainingDataList(input);
        }

        [HttpGet]
        public Task<StudentGeneralTrainingDataDetails> GetGeneralTrainingDataDetails(int studentId, int classId)
        {
            return _studentLODService.GetGeneralTrainingDataDetails(studentId, classId);
        }

        [HttpGet]
        public Task<List<VCurriculumStudentDetail>> GetCurriculumDetailsByStudentClass(int studentClassId)
        {
            return _studentLODService.GetCurriculumDetailsByStudentClass(studentClassId);
        }

        [HttpPost]
        public async Task<FileContentResult> GeneratePersonalFile(LodGeneratorModel model, CancellationToken cancellationToken)
        {
            var fileContents = await _studentLODService.GeneratePersonalFile(model, cancellationToken);
            return File(fileContents, "application/docx", $"{model?.PersonId}.docx");
        }

        [HttpPost]
        public async Task<FileContentResult> GeneratePersonalFileForStay(LodGeneratorModel model, CancellationToken cancellationToken)
        {
            var fileContents = await _studentLODService.GeneratePersonalFileForStayAsync(model, cancellationToken);
            return File(fileContents, "application/docx", $"{model?.PersonId}.docx");
        }

        [HttpGet]
        public async Task<IPagedList<LodFinalizationViewModel>> List([FromQuery] LodFinalizationListInput input, CancellationToken cancellationToken)
        {
            var result = await _studentLODService.ListFinalizations(input, cancellationToken);

            return result;
        }
    }
}
