namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Update;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResourceSupportService
    {
        Task<StudentResourceSupportModel> GetById(int id);
        Task<List<StudentResourceSupportViewModel>> GetByPersonId(int personId);
        Task Create(StudentResourceSupportModel model);
        Task Update(StudentResourceSupportModel model);
        Task Delete(int id);
        Task<StudentResourceSupportViewModel> ChechForExistingByPerson(int personId, int schoolYear);
        Task UpdateStudentResourceSupport(StudentResourceSupportUpdateModel mode);
        Task<ResourceSupportViewModel> GetStudentResourceSupport(int personId, int? schoolYea);
    }
}
