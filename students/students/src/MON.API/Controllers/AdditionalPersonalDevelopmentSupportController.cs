namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.PersonalDevelopmentSupport;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class AdditionalPersonalDevelopmentSupportController : BaseApiController
    {
        private readonly IAdditionalPersonalDevelopmentSupportService _service;

        public AdditionalPersonalDevelopmentSupportController(IAdditionalPersonalDevelopmentSupportService service)
        {
                _service = service;
        }

        [HttpGet]
        public async Task<AdditionalPersonalDevelopmentSupportViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public Task<IEnumerable<SopDetailsViewModel>> GetSopForPerson([FromQuery] int personId, [FromQuery] int schoolYear, CancellationToken cancellationToken)
        {
            return _service.GetSopForPerson(personId, schoolYear, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<AdditionalPersonalDevelopmentSupportViewModel>> List([FromQuery] StudentListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<VStudentEplrHoursTaken>> ListSchoolBookData([FromQuery] OresListInput input, CancellationToken cancellationToken)
        {
            return await _service.ListSchoolBookData(input, cancellationToken);
        }

        [HttpPost]
        public async Task Create(AdditionalPersonalDevelopmentSupportModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(AdditionalPersonalDevelopmentSupportModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }

        [HttpPut]
        public async Task SuspendAdditionalPersonalDevelopmentSupport(AdditionalPersonalDevelopmentSupportISuspendtemModel model, CancellationToken cancellationToken)
        {
            await _service.SuspendAdditionalPersonalDevelopmentSupport(model, cancellationToken);
        }
    }
}
