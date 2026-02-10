using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Models.StudentModels.Lod;
using MON.Models.StudentModels.StoredProcedures;
using MON.Report.Model;
using MON.Services.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class RelocationDocumentController : BaseApiController
    {
        private readonly IRelocationDocumentService _service;
        public RelocationDocumentController(IRelocationDocumentService service, ILogger<RelocationDocumentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<RelocationDocumentModel>> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);  
        }

        [HttpGet]
        public Task<List<RelocationDocumentViewModel>> GetByPersonId(int personId, CancellationToken cancellationToken)
        {
            return _service.GetByPersonId(personId, cancellationToken);
        }

        [HttpGet]
        public async Task<IEnumerable<RelocationDocumentOptionsModel>> GetRelocationDocumentOptionsByPerson(int personId, CancellationToken cancellationToken)
        {
            return await _service.GetRelocationDocumentOptionsByPerson(personId, cancellationToken);
        }

        [HttpGet]
        public Task<StudentTermGradeViewModel> GetStudentCurrentTermGrades(int relocationDocumentId, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken)
        {
            return _service.GetStudentCurrentTermGradesAsync(relocationDocumentId, filterForCurrentInstitution, filterForCurrentSchoolBook, cancellationToken);
        }

        [HttpGet]
        public Task<RelocationDocumentAbsencePrintModel> GetAbsences(int relocationDocumentId, CancellationToken cancellationToken)
        {
            return _service.GetAbsences(relocationDocumentId, cancellationToken);
        }

        [HttpPost]
        public async Task Create(RelocationDocumentModel model)
        {
            await _service.Create(model);  
        }

        [HttpPut]
        public async Task Update(RelocationDocumentModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int documentId)
        {
            await _service.Delete(documentId);
        }

        [HttpPut]
        public async Task Confirm(int id)
        {
            await _service.Confirm(id);
        }

        [HttpGet]
        public async Task<IEnumerable<StudentLodAssessmentListModel>> GetLodAssessmentsList(int relocationDocumentId, CancellationToken cancellationToken)
        {
            return await _service.GetLodAssessmentsList(relocationDocumentId, cancellationToken);
        }
    }
}
