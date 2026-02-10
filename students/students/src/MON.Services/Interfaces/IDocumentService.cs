using MON.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentViewModel> GetDocumentByIdAsync(int id);
        Task DeleteResourceSupportDocumentByIdAsync(int documentId);
        Task<List<DocumentViewModel>> TestFileManager();
        Task<bool> CheckForExistingAdmissionDocumentAsync(int personId, int institutionId);
        Task<bool> CheckForAdmissionDocumentInTheSameInstitutionAsync(int personId, int institutionId);
    }
}
