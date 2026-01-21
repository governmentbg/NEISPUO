namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Grid;
    using MON.Models.Institution;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IInstitutionService
    {
        HashSet<int> ExternalSoProviderClassTypeEnrollmentLimitation { get; }
        bool ExternalSoProviderLimitationsCheck { get; }

        public Task<short> GetCurrentYear(int? institutionId);
        Task<InstitutionDropdownViewModel> GetDropdownModelByIdAsync(int institutionId);
        Task<IPagedList<ClassGroupBaseModel>> GetClassGroupsForInstitutionAsync(ClassesListInput input);
        Task AutoNumberAsync(int classId);
        Task<InstitutionCacheModel> GetByIdAsync(int? institutionId);
        Task<List<InstitutionInfoModel>> GetInstitutionsAsync();
        Task<List<DropdownViewModel>> GetInstitutionsDropdownModelAsync();
        Task<IPagedList<StudentInfoExternalModel>> ListInstitutionExternalDetails(InstitutionListInput input);
        Task<IPagedList<InstitutionDetailsModel>> List(InstitutionListInput input);
        Task<IPagedList<StudentInfoFullModel>> ListStudents(InstitutionListInput input);
        Task<int?> GetCurrentForStudent(int? studentId);
        Task<InstitutionDetailsModel> GetFullDetails(int? id);
        Task<bool> CanManageInstitution(int institutionId);
        Task<bool> CanManageClass(int classId);
        Task<int> GetInstitutionDetailedSchoolTypeId(int? institutionId);
        Task<InstitutionDropdownViewModel> GetLoggedUserInstitution();
        Task<List<ClassGroupDropdownViewModel>> GetClassGroupsForAdditionalEnrollment(int personId, short schoolYear, string selectedValues);

        /// <summary>
        /// Позволени Позиции в зависимост от типа на институцията.
        /// Таблица student.AppSettings, ключ InstTypeToPositionLimit.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        Task<HashSet<int>> GetAllowedStudentPositions(int institutionId);

        /// <summary>
        /// Позволени видове паралелки в зависимост от типа на институцията при запис в група/паралелка.
        /// Таблица student.AppSettings, ключ InstTypeToClassKindEnrollmentLimit.
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        Task<InstTypeToClassTypeConfiguration> GetAllowedClasssKindsEnrollmentLimit(int institutionId);

        /// <summary>
        /// Връща <see cref="InstitutionCacheModel"/> кеширан за 1 час
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        Task<InstitutionCacheModel> GetInstitutionCache(int institutionId);

        Task<bool> HasExternalSoProvider(int institutionId, int schoolYear);
        Task<bool> HasExternalSoProviderForLoggedInstitution();
    }
}
