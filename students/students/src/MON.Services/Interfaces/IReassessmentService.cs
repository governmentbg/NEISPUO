namespace MON.Services.Interfaces
{
    using MON.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IReassessmentService
    {
        Task<List<ReassessmentModel>> GetListForPersonAsync(int personId);

        Task<ReassessmentModel> GetByIdAsync(int id);

        Task Create(ReassessmentModel model);

        Task Update(ReassessmentModel model);

        Task Delete(int id);

    }
}
