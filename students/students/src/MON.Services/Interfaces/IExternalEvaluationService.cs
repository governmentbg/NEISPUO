using MON.Models.ExternalEvaluation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{

    public interface IExternalEvaluationService
    {
        Task<List<ExternalEvaluationModel>> GetByPersonId(int personId);
        Task Create(ExternalEvaluationModel model);
        Task Update(ExternalEvaluationModel model);
        Task Delete(int id);
    }
}
