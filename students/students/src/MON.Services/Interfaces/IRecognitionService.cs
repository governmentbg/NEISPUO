using MON.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{

    public interface IRecognitionService
    {
        Task<RecognitionModel> GetById(int id);
        Task<List<RecognitionViewModel>> GetListForPerson(int personId);
        Task Delete(int id);
        Task Update(RecognitionModel model);
        Task Create(RecognitionModel model);
        Task<string> GetRecognitionRequiredSubjects();
    }
}
