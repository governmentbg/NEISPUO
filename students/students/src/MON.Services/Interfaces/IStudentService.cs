using MON.Models;
using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Search;
using MON.Models.StudentModels.Update;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IStudentService : IGenericRepository<StudentCreateModel, StudentViewModel, StudentBasicDetailsUpdateModel>
    {
        Task<IPagedList<StudentSearchViewModel>> GetBySearch(StudentSearchModelExtended studentSearchModel);
        Task<StudentSummaryModel> GetSummaryByIdAsync(int id, CancellationToken cancellationToken);
        Task<IPagedList<StudentSearchViewModel>> ListAsync(StudentListInput input, CancellationToken cancellationToken);
        Task<IPagedList<StudentEmployerDeatilsListModel>> EmployerDetailsList(DualFormEmployerListInput input, GridFilter[] filters, CancellationToken cancellationToken);
        Task<StudentSearchViewModel> CheckPinUniqueness(string pin);
        Task UpdateInternationalProtection(StudentAdditionalDetailsModel model);
        Task<StudentPersonalDataModel> GetPersonDataByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<EducationViewModel>> GetStudentEducationAsync(int personId);
        Task<StudentAdditionalDetailsModel> GetStudentInternationalProtectionAsync(int personId);
        Task<bool> CanManageStudent(int personId);

        /// <summary>
        /// Проверява дали даден Person е ученик в институцията на логнатия потребител.
        /// За институция от тип 1 или 2 трябва да има записи в EducationalState с позиция 3 или 10.
        /// За институция от тип 3 или 5 трябва да има записи в EducationalState с позиция 8.
        /// За институция от тип 4 трябва да има записи в EducationalState с позиция 7.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<bool> IsStudentInCurrentInstitution(int personId);

        /// <summary>
        /// Проверява дали даден Person е ученик в дадена институция.
        /// За институция от тип 1 или 2 трябва да има записи в EducationalState с позиция 3 или 10.
        /// За институция от тип 1 или 2 и документ за отписване, трябва да има записи в EducationalState с позиция 3, 8 или 10.
        /// За институция от тип 3 или 5 трябва да има записи в EducationalState с позиция 8.
        /// За институция от тип 4 трябва да има записи в EducationalState с позиция 7.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="institutionId"></param>
        /// <param name="isDischarge"></param>
        /// <returns></returns>
        Task<bool> IsStudentInInstitution(int personId, int institutionId, bool isDischarge = false);
        Task<bool> CanEditStudentPersonalDetails(int personId);
        Task DeleteAzureAccount(int studentId);

        /// <summary>
        /// Връща институцията, която реално посещава.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="submittedInstitutionId"></param>
        /// <returns></returns>
        Task<int> GetAttendedInstitution(int personId, int submittedInstitutionId);

        Task<string> GetCurrentClassName(int personId, int schoolYear);
        
        Task<StudentClassViewModel> GetCurrentClass(int personId, int schoolYear);
        Task<StudentClassViewModel> GetCurrentClass(int personId);
        Task<InitialEnrollmentVisibilityCheckResultModel> InitialEnrollmentSecretBtnVisibilityCheck(int personId);

        /// <summary>
        /// Връща основната (учебна) паралелка на ученик за дадена учебна година.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        Task<StudentClassViewModel> GetMainStudentClass(int personId, bool? isCurrent = true, int? schoolYear = null, int? institutionId = null);

        /// <summary>
        /// Връща основните (учебни) паралелка на ученик за дадена учебна година.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        Task<List<StudentClassViewModel>> GetMainStudentClasses(int personId, int? schoolYear = null, bool? includeIsnotPresentForm = null);
    }
}
