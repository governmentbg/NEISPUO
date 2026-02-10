namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.Dropdown;
    using MON.Models.Grid;
    using MON.Models.Ores;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class OresController : BaseApiController
    {
        private readonly IOresService _service;

        public OresController(IOresService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<OresViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<OresViewModel>> List([FromQuery] OresListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpPost]
        public async Task Create(OresModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(OresModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }

        [HttpGet]
        public async Task<OresDetailsViewModel[]> GetCalendarDetails(DateTime start, DateTime end, int? regionId, int? institutionId, CancellationToken cancellationToken)
        {
            return await _service.GetCalendarDetails(start, end, regionId, institutionId, cancellationToken);
        }

        [HttpGet]
        public async Task<OresRangeDropdownViewModel[]> GetOresRangeDropdownOptions([FromQuery] OresListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetOresRangeDropdownOptions(input, cancellationToken);
        }
    }
}
