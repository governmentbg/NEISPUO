namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models.ExternalEvaluation;
    using MON.Models.Finance;
    using MON.Models.Grid;
    using MON.Models.HealthInsurance;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Authorize]
    public class FinanceController : BaseApiController
    {
        private readonly IFinanceService _service;

        public FinanceController(IFinanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<List<Dictionary<string, object>>> GetNaturalIndicators(short schoolYear, int period)
        {
            return _service.GetNaturalIndicators(schoolYear, period);
        }

        [HttpGet]
        public Task<IPagedList<Dictionary<string, object>>> ListPeriod([FromQuery] NaturalIndicatorsDataListInput input)
        {
            return _service.ListPeriod(input);
        }

        [HttpGet]
        public Task<List<GridHeaderModel>> GetGridHeaders()
        {
            return _service.GetGridHeaders();
        }

        [HttpGet]
        public Task<List<GridHeaderModel>> GetResourceSupportDataGridHeaders()
        {
            return _service.GetResourceSupportDataGridHeaders();
        }

        [HttpGet]
        public Task<IPagedList<Dictionary<string, object>>> ListPeriods([FromQuery] NaturalIndicatorsDataListInput input)
        {
            return _service.ListPeriods(input);
        }

        [HttpGet]
        public Task<IPagedList<Dictionary<string, object>>> ListResourceSupportDataPeriods([FromQuery] ResourceSupportDataDataListInput input)
        {
            return _service.ListResourceSupportDataPeriods(input);
        }
    }
}
