namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.PreSchool;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class PreSchoolEvaluationController : BaseApiController
    {
        private readonly IPreSchoolEvaluationService _service;

        public PreSchoolEvaluationController(IPreSchoolEvaluationService preSchoolEvaluation)
        {
            _service = preSchoolEvaluation;
        }

        [HttpGet]
        public async Task<IPagedList<PreSchoolEvaluationViewModel>> List([FromQuery] PreSchoolEvalListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public Task<List<PreSchoolEvaluationViewModel>> GetByPersonId(int personId, CancellationToken cancellationToken)
        {
            return _service.GetByPersonId(personId, cancellationToken);
        }

        [HttpGet]
        public async Task<PreSchoolEvaluationViewModel> GetEvalById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetEvalById(id, cancellationToken);
        }

        [HttpGet]
        public Task<PreSchoolReadinessModel> GetReadinessForFirstGrade(int personId, CancellationToken cancellationToken)
        {
            return _service.GetReadinessForFirstGradeAsync(personId, cancellationToken);
        }

        [HttpPost]
        public Task CreateForBasicClass(PreSchoolEvaluationModel model)
        {
            return _service.CreateForBasicClass(model);
        }

        [HttpGet]
        public async Task ImportFromSchoolBook(int personId, int basicClassId, short? schoolYear)
        {
            await _service.ImportFromSchoolBook(personId, basicClassId, schoolYear);
        }

        [HttpPost]
        public Task CreateReadinessForFirstGrade(PreSchoolEvaluationModel model)
        {
            return _service.CreateReadinessForFirstGrade(model);
        }

        [HttpPut]
        public Task Update(PreSchoolEvaluationModel model)
        {
            return _service.Update(model);
        }

        [HttpPut]
        public Task UpdateReadinessForFirstGrade(PreSchoolReadinessModel model)
        {
            return _service.UpdateReadinessForFirstGradeAsync(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }

        [HttpDelete]
        public Task DeleteReadiness(int id)
        {
            return _service.DeleteReadiness(id);
        }
    }
}
