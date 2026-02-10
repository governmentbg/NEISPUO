using MON.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IAdmissionDocumentService
    {
        Task<AdmissionDocumentViewModel> GetById(int id);
        Task<IEnumerable<AdmissionDocumentViewModel>> GetByPersonId(int personId);
        Task Create(AdmissionDocumentModel admissionDocumentModel);
        Task Update(AdmissionDocumentModel admissionDocumentModel);
        Task Delete(int documentId);
        Task Confirm(int id);
        Task<bool> CheckForExistingAdmissionDocumentAsync(int personId, int institutionId);
        Task<bool> CheckForAdmissionDocumentInTheSameInstitutionAsync(int personId, int institutionId);
        Task<List<AdmissionDocumentViewModel>> GetListForRelocationDocument(int relocationDocumentId);
    }
}
