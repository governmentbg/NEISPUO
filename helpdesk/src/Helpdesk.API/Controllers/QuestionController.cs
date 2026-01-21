namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Issue;
    using Helpdesk.Models.Question;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class QuestionController : BaseApiController
    {
        private readonly IQuestionService _service;
        private readonly IAuthenticationService _authenticationService;

        public QuestionController(IQuestionService issueService, IAuthenticationService authenticationService)
        {
            _service = issueService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public Task<IPagedList<QuestionViewModel>> List([FromQuery] QuestionPageListInput input)
        {
            return _service.GetListAsync(input);
        }

        [HttpGet]
        public async Task<QuestionViewModel> GetById(int id)
        {
            if (!await _authenticationService.DemandPermissionsForQuestionAsync(id, PermissionEnum.Read))
            {
                throw new UnauthorizedAccessException();
            }
            return await _service.GetById(id);
        }

        [HttpGet]
        public async Task<QuestionViewModel> GetEditModelById(int id)
        {
            if (!await _authenticationService.DemandPermissionsForQuestionAsync(id, PermissionEnum.Update))
            {
                throw new UnauthorizedAccessException();
            }
            return await _service.GetEditModelById(id);
        }

        [HttpPost]
        public async Task<int> Create(QuestionModel model)
        {
            if (!await _authenticationService.DemandPermissionsForQuestionAsync(model.Id, PermissionEnum.Create))
            {
                throw new UnauthorizedAccessException();
            }
            return await _service.Create(model);
        }

        [HttpPut]
        public async Task Update(QuestionModel model)
        {
            if (!await _authenticationService.DemandPermissionsForQuestionAsync(model.Id, PermissionEnum.Update))
            {
                throw new UnauthorizedAccessException();
            }
            await _service.Update(model);
        }

    }
}
