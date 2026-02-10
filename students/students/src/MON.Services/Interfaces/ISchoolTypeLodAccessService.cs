using MON.Models.SchoolTypeLodAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface ISchoolTypeLodAccessService
    {
        Task<IEnumerable<SchoolTypeLodAccessModel>> GetAllAsync();
        Task<SchoolTypeLodAccessModel> GetByIdAsync(int detailedSchoolTypeId);
        Task UpdateAsync(SchoolTypeLodAccessModel model);
    }
}
