namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.Grid;
    using MON.Models.Refugee;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public interface IRefugeeService
    {
        Task<IPagedList<ApplicationViewModel>> ApplicationList(RefugeeApplicationListInput input);

        Task<int> CreateApplication(ApplicationModel model);

        Task<ApplicationModel> GetByIdAsync(int id);
        Task<ApplicationViewModel> GetDetailsByIdAsync(int id);

        Task UpdateApplication(ApplicationModel noteModel);
        Task DeleteApplication(int applicationId);
        Task DeleteApplicationChild(int applicationChildId);
        Task<int> CountPendingAdmissions();
        Task<IPagedList<RefugeeAdmissionViewModel>> AdmissionList(PagedListInput input);
        Task CompleteApplication(int applicationId);
        Task CompleteApplicationChild(int applicationChildId);
        Task CancelApplication(RefugeeApplicationCancellationModel model);
        Task CancelApplicationChild(RefugeeApplicationCancellationModel model);
        Task UnlockApplication(int applicationId);
        Task UnlockApplicationChild(int applicationChildId);
    }
}
