namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.PreSchool;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPreSchoolEvaluationService
    {
        Task<IPagedList<PreSchoolEvaluationViewModel>> List(PreSchoolEvalListInput input, CancellationToken cancellationToken);
        Task<List<PreSchoolEvaluationViewModel>> GetByPersonId(int personId, CancellationToken cancellationToken);
        Task<PreSchoolEvaluationViewModel> GetEvalById(int id, CancellationToken cancellationToken);
        Task<PreSchoolReadinessModel> GetReadinessForFirstGradeAsync(int personId, CancellationToken cancellationToken);
        Task ImportFromSchoolBook(int personId, int basicClassId, short? schoolYear);
        Task CreateForBasicClass(PreSchoolEvaluationModel model);
        Task CreateReadinessForFirstGrade(PreSchoolEvaluationModel model);
        Task Update(PreSchoolEvaluationModel model);
        Task UpdateReadinessForFirstGradeAsync(PreSchoolReadinessModel model);
        Task Delete(int id);
        Task DeleteReadiness(int id);
    }
}
