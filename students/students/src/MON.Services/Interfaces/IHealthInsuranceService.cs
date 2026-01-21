using MON.Models;
using MON.Models.Grid;
using MON.Models.HealthInsurance;
using MON.Shared.Interfaces;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IHealthInsuranceService
    {
        Task<IPagedList<HealthInsuranceStudentsViewModel>> List(HealthInsuranceDataListInput input);
        Task<IPagedList<HealthInsuranceExportFileModel>> ExportsList(HealthInsuranceDataListInput input);
        Task GenerateHealthInsuranceFile(HealthInsuranceStudentsFileModel model);
        Task Delete(int id);
    }
}
