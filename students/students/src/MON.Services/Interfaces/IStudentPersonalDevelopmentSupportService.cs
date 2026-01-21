namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStudentPersonalDevelopmentSupportService
    {
        Task<PersonalDevelopmentSupportModel> GetById(int id);
        Task<List<PersonalDevelopmentSupportViewModel>> GetListForPerson(int personId);
        Task Create(PersonalDevelopmentSupportModel model);
        Task Update(PersonalDevelopmentSupportModel model);
        Task Delete(int id);
    }
}
