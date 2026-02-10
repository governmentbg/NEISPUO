using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MON.Models;
using MON.Models.Grid;
using MON.Models.HealthInsurance;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class HealthInsuranceController : BaseApiController
    {
        private readonly IHealthInsuranceService _service;

        public HealthInsuranceController(IHealthInsuranceService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IPagedList<HealthInsuranceStudentsViewModel>> List([FromQuery] HealthInsuranceDataListInput input)
        {
            return _service.List(input);
        }

        [HttpGet]
        public Task<IPagedList<HealthInsuranceExportFileModel>> ExportsList([FromQuery] HealthInsuranceDataListInput input)
        {
            return _service.ExportsList(input);
        }

        [HttpPost]
        public async Task GenerateHealthInsuranceFile(HealthInsuranceStudentsFileModel model)
        {
            await _service.GenerateHealthInsuranceFile(model);
        }

        [HttpDelete]
        public Task Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
