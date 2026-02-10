using MON.Models;
using MON.Models.StudentModels;
using MON.Models.StudentModels.Class;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IStudentClassService
    {
        Task<StudentClassViewModel> GetById(int id);
        Task<List<StudentClassViewModel>> GetHistoryById(int id);
        Task<List<StudentClassViewModel>> GetByPersonId(StudentClassesTimelineInputModel input);
        Task<List<StudentClassViewModel>> GetMainForPersonAndLoggedInstitution(int personId, short schoolYear);
        Task<List<ClassGroupDropdownViewModel>> GetDropdownOptionsForLoggedInstitution(int personId, short? schoolYear);
        Task<List<PersonBasicStudentClassDetails>> GetPersonBasicClasses(int personId, bool? forCurrentInstitution);
        Task<StudentClassSummaryModel> GetCurrentClassSummaryByIdAsync(int studentClassId);
        Task EnrollInClass(StudentClassModel model, bool withStudentEduForm);
        Task<int> ChangeStudentClass(StudentClassModel model, bool withStudentEduForm);
        Task Update(StudentClassModel model);
        Task DeleteHistoryRecord(int id);
        Task<bool> AddToNewClassBtnVisibilityCheck(int personId);
        Task EnrollInAdditionalClass(StudentClassBaseModel model);
        Task EnrollInCplrAdditionalClass(StudentClassModel model);
        Task UnenrollFromClass(StudentClassUnenrollmentModel model);
        Task UpdateAdditionalClass(StudentAdditionalClassChangeModel model);
        Task<int> ChangeAdditionalClass(StudentAdditionalClassChangeModel model);
        Task Delete(int id);
        Task EnrollInCplrClass(StudentClassModel model);
        Task UnenrollSelected(StudentClassMassUnenrollmentModel model);
        Task EnrollSelected(StudentClassMassEnrollmentModel model);
        Task<StudentClassDualFormCompanyModel[]> GetDualFormCompanies(int studentClassId, CancellationToken cancellationToken);
        Task ChangePosition(StudentPositionChangeModel model);
    }
}
