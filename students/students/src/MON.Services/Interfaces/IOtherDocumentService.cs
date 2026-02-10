using MON.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IOtherDocumentService
    {
        Task<OtherDocumentModel> GetById(int id);
        Task<List<OtherDocumentModel>> GetByPersonId(int personId);
        Task Delete(int id);
        Task Update(OtherDocumentModel model);
        Task Create(OtherDocumentModel model);
    }
}
