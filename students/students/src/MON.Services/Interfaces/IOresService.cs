namespace MON.Services.Interfaces
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Dropdown;
    using MON.Models.Grid;
    using MON.Models.Ores;
    using MON.Shared.Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IOresService
    {
        Task<OresViewModel> GetById(int id, CancellationToken cancellationToken);
        Task<IPagedList<OresViewModel>> List(OresListInput input, System.Threading.CancellationToken cancellationToken);
        Task Create(OresModel model);
        Task Update(OresModel model);
        Task Delete(int id);
        Task<OresDetailsViewModel[]> GetCalendarDetails(DateTime start, DateTime end, int? regionId, int? institutionId, System.Threading.CancellationToken cancellationToken);
        Task<OresRangeDropdownViewModel[]> GetOresRangeDropdownOptions([FromQuery] OresListInput input, System.Threading.CancellationToken cancellationToken);
    }
}
