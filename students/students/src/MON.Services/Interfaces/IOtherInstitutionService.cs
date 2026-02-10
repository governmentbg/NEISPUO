using MON.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IOtherInstitutionService
    {
        Task<List<OtherInstitutionViewModel>> GetStudentOtherInstitutions(int personId);
        Task DeleteAsync(int otherInstitutionId);
        Task CreateAsync(OtherInstitutionViewModel model);
        Task<OtherInstitutionViewModel> GetByIdAsync(int institutionId);
        Task UpdateAsync(OtherInstitutionViewModel model);
    }
}
