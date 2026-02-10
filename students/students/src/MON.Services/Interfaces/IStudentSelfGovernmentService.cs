namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStudentSelfGovernmentService
    {
        Task<SelfGovernmentModel> GetById(int selfGovernmentId);
        Task<List<SelfGovernmentViewModel>> GetByPersonId(int personId);
        Task Create(SelfGovernmentModel model);
        Task Update(SelfGovernmentModel model);
        Task Delete(int selfGovernmentId);
    }
}
