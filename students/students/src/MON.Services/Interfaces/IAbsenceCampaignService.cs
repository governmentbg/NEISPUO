using MON.ASPDataAccess;
using MON.DataAccess;
using MON.Models;
using MON.Models.Absence;
using MON.Models.ASP;
using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IAbsenceCampaignService
    {
        Task<IPagedList<AbsenceCampaignViewModel>> List(PagedListInput input, CancellationToken cancellationToken);
        Task<AbsenceCampaignInputModel> GetById(int id, CancellationToken cancellationToken);
        Task Create(AbsenceCampaignInputModel model);
        Task Update(AbsenceCampaignInputModel model);
        Task ToggleManuallyActivation(AbsenceCampaignInputModel model);
        Task<List<AbsenceCampaignViewModel>> GetActive(CancellationToken cancellationToken);
        Task<AbsenceCampaignViewModel> GetDetailsById(int id, CancellationToken cancellationToken);
        Task<IPagedList<AbsenceReportViewModel>> AbsenceReportsList(AbsenceReportListInput input, CancellationToken cancellationToken);
        Task SendNotification(AbsenceCampaign entity);
        Task Delete(int id);
        Task<List<KeyValuePair<string, int>>> GetStats(int id);
        Task<AspSessionInfoViewModel> GetAspSession(short schoolYear, short month, string infoType, CancellationToken cancellationToken);
        Task<IPagedList<AspStudentGridItemModel>> GetASPStudentsForCampaign(int campaignId, PagedListInput input, CancellationToken cancellationToken);
        Task<IPagedList<AspMonAbsenceViewModel>> GetAspAbsencesForCampaign(int campaignId, PagedListInput input, CancellationToken cancellationToken);
        Task<IPagedList<AspMonZpViewModel>> GetAspZpForCampaign(int campaignId, AspMonZpListInput input, CancellationToken cancellationToken);
    }
}
