using MON.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IDischargeDocumentService
    {
        Task<DischargeDocumentModel> GetById(int id);
        Task<List<DischargeDocumentModel>> GetByPersonId(int personId);
        Task Delete(int id);
        Task Update(DischargeDocumentModel model);
        Task Confirm(int id);
        Task Create(DischargeDocumentModel model);
        Task<int?> GetCurrentStudentClass(int personId, int institutionId, int? selectedStudentClassId = null);
    }
}
