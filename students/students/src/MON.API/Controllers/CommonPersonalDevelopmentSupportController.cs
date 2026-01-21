namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Models.StudentModels.PersonalDevelopmentSupport;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Threading;
    using System.Threading.Tasks;

    public class CommonPersonalDevelopmentSupportController : BaseApiController
    {
        private readonly ICommonPersonalDevelopmentSupportService _service;

        public CommonPersonalDevelopmentSupportController(ICommonPersonalDevelopmentSupportService service)
        {
                _service = service;
        }

        [HttpGet]
        public async Task<CommonPersonalDevelopmentSupportViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            return await _service.GetById(id, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<CommonPersonalDevelopmentSupportViewModel>> List([FromQuery] StudentListInput input, CancellationToken cancellationToken)
        {
            return await _service.List(input, cancellationToken);
        }

        [HttpPost]
        public async Task Create(CommonPersonalDevelopmentSupportModel model)
        {
            await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(CommonPersonalDevelopmentSupportModel model)
        {
            await _service.Update(model);
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            await _service.Delete(id);
        }
    }
}
