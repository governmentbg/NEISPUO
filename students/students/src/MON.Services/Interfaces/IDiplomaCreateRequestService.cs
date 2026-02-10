using MON.Models;
using MON.Models.Diploma;
using MON.Models.StudentModels;
using MON.Shared.Interfaces;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IDiplomaCreateRequestService
    {
        Task<IPagedList<DiplomaCreateRequestViewModel>> List(PagedListInput input);
        Task<DiplomaCreateRequestModel> GetById(int id);
        Task Create(DiplomaCreateRequestModel model);
        Task Update(DiplomaCreateRequestModel model);
        Task Delete(int id);
    }
}
