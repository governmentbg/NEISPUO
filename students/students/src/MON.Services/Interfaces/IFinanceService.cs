namespace MON.Services.Interfaces
{
    using MON.Models.Finance;
    using MON.Models.Grid;
    using MON.Models.HealthInsurance;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFinanceService
    {
        Task<List<Dictionary<string, object>>> GetNaturalIndicators(short schoolYear, int period);
        Task<IPagedList<Dictionary<string, object>>> ListPeriod(NaturalIndicatorsDataListInput input);
        Task<IPagedList<Dictionary<string, object>>> ListPeriods(NaturalIndicatorsDataListInput input);
        Task<IPagedList<Dictionary<string, object>>> ListResourceSupportDataPeriods(ResourceSupportDataDataListInput input);
        Task<List<GridHeaderModel>> GetGridHeaders();
        Task<List<GridHeaderModel>> GetResourceSupportDataGridHeaders();
    }
}
