using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Shared.Interfaces;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IAdmissionPermissionRequestService
    {
        Task<AdmissionPermissionRequestModel> GetById(int id);
        Task<IPagedList<AdmissionPermissionRequestViewModel>> List(AdmissionDocRequestListInput input);
        Task<int> CountPending();
        Task Create(AdmissionPermissionRequestModel model);
        Task Update(AdmissionPermissionRequestModel model);
        Task Delete(int id);
        Task Confirm(int id);
    }
}
