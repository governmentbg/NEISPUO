namespace MON.Services.Interfaces
{
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;
    using MON.Models.StudentModels.Lod;
    using MON.Models.Grid;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using MON.Models;
    using System.Threading;
    using MON.DataAccess;

    /// <summary>
    /// ЛОД/Оценки представлява агрегиран изглед на срочните и годишните оценки от дневника, приравняване, признаване и самостоятелна форма на обучение.
    /// За самостоятелната форма на обучение следва да има интерфейс за управление в ЛОД-а на ученика, тъй като си нямат дневник.
    /// </summary>
    public interface ILodAssessmentService
    {
        /// <summary>
        /// Връща списък с учебните години и випускте, за който има записи в агрегираните срочни и годишни оценки.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedList<StudentLodAssessmentListModel>> List(LodEvaluationListInput input, CancellationToken cancellationToken = default);
        Task<IPagedList<LodAssessmentImportListModel>> ListImported(LodAssessmentListInput input, CancellationToken cancellationToken = default);

        Task<List<LodAssessmentCurriculumPartModel>> GetPersonAssessments(int personId, int basicClass, int schoolYear, bool isSelfEduForm, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken = default);
        Task<List<LodAssessmentCurriculumPartModel>> GetPersonCurrentAssessments(int personId, int basicClass, int schoolYear, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken = default);
        Task<List<LodAssessmentCurriculumPartModel>> GetStudentClassCurriculum(int studentClassId, CancellationToken cancellationToken);
        Task Import(IFormFile file);
        Task<LodAssessmentImportValidationModel> ValidateImport(IFormFile file, CancellationToken cancellationToken = default);
        Task CreateOrUpdate(List<LodAssessmentCurriculumPartModel> model);
        Task ModulesWithoutProfSubjectFixture(List<VStudentLodAsssessment> asssessments);

        Task<List<ClassGroupDropdownViewModel>> GetMainStudentClasses(int personId, CancellationToken cancellationToken = default);
        Task Delete(int personId, int basicClass, int schoolYear, bool isSelfEduForm, int institutionId, CancellationToken cancellationToken = default);
        byte[] ExportFile(LodAssessmentImportValidationModel model);
    }
}
