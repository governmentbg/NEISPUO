namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStudentAwardService
    {
        Task<StudentAwardModel> GetById(int id);
        Task<IEnumerable<StudentAwardViewModel>> GetByPersonId(int personId);
        Task Create(StudentAwardModel model);
        Task Update(StudentAwardModel model);
        Task Delete(int awardId);
    }
}
