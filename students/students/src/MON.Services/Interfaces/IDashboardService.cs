namespace MON.Services.Interfaces
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.ASP;
    using MON.Models.Dashboards;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDashboardService
    {
        Task<DirectorDashboardModel> GetDirectorDashboardAsync(int? institutionId);
        Task AdmissionStudentsAsync(List<StudentClassModel> models);
        Task DischargeStudentsAsync(List<StudentToBeDischargedModel> models);
        Task<IPagedList<StudentForAdmissionModel>> GetStudentsForAdmission(PagedListInput input);
        Task<IPagedList<SopEnrollmentDetailViewModel>> GetSopEnrollmentDetails(PagedListInput input);
        Task<int> GetInstitutiontSopEnrollmentsCount();
        Task<IPagedList<StudentToBeDischargedModel>> GetStudentsForDischarge(PagedListInput input);
        Task<int> GetStudentsCount();
        Task<List<StudentStatsModel>> GetStudentsCountGroupByClassType();
        Task<List<StudentStatsModel>> GetStudentsCountByClassType(int classTypeId);
        Task<ClassGroupStatsModel> GetClassGroupStats();
        Task<DiplomaStatsModel> GetDiplomaStats();
        Task<List<AbsenceCampaignViewModel>> GetAllActiveCampaigns();
        Task<IPagedList<StudentEnvironmentCharacteristicModel>> GetStudentEnvironmentCharacteristics(StudentEnvCharacteristicListInput input, CancellationToken cancellationToken);
        Task<IPagedList<VStudentExternalEvaluation>> GetStudentExernalEvaluationsList(DualFormEmployerListInput input, CancellationToken cancellationToken);
    }
}
